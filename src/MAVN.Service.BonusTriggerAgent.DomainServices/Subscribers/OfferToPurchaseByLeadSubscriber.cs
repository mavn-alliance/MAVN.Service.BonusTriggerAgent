using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.BonusTriggerAgent.Contract.Events;
using MAVN.Service.BonusTriggerAgent.Domain;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.CurrencyConvertor.Client.Models.Enums;
using Lykke.Service.Referral.Contract.Events;

namespace MAVN.Service.BonusTriggerAgent.DomainServices.Subscribers
{
    public class OfferToPurchaseByLeadSubscriber
        : RabbitSubscriber<OfferToPurchaseByLeadEvent>
    {
        private readonly string _assetName;
        private readonly IRabbitPublisher<BonusTriggerEvent> _bonusTriggerEventPublisher;
        private readonly ICurrencyConvertorClient _currencyConverterClient;

        public OfferToPurchaseByLeadSubscriber(
            string connectionString,
            string exchangeName,
            string assetName,
            ILogFactory logFactory,
            IRabbitPublisher<BonusTriggerEvent> bonusTriggerEventPublisher,
            ICurrencyConvertorClient currencyConverterClient
            )
            : base(connectionString, exchangeName, logFactory)
        {
            _bonusTriggerEventPublisher = bonusTriggerEventPublisher ??
                                          throw new ArgumentNullException(nameof(bonusTriggerEventPublisher));
            _currencyConverterClient = currencyConverterClient ??
                                       throw new ArgumentNullException(nameof(currencyConverterClient));
            _assetName = assetName;
        }

        public override async Task<bool> ProcessMessageAsync(OfferToPurchaseByLeadEvent message)
        {
            if (string.IsNullOrWhiteSpace(message.CurrencyCode))
            {
                Log.Error(null, "Currency is missing in Purchase Referral event", context: message, process: nameof(ProcessMessageAsync));
                return false;
            }

            var amount = 0M;
            if (message.NetPropertyPrice.HasValue)
            {
                amount = message.NetPropertyPrice.Value;
            }

            if (amount < 0)
            {
                Log.Error(null, "Amount has invalid value in Purchase Referral Event", context: message, process: nameof(ProcessMessageAsync));
                return false;
            }

            var response = await _currencyConverterClient.Converter
                .ConvertAsync(message.CurrencyCode, _assetName, amount);

            if (response.ErrorCode != ConverterErrorCode.None)
            {
                Log.Error(message: "An error occured while converting currency amount",
                    context: $"from: {message.CurrencyCode}; to: {_assetName}; error: {response.ErrorCode}");

                return false;
            }

            await _bonusTriggerEventPublisher.PublishAsync(new BonusTriggerEvent
            {
                CustomerId = message.AgentId,
                TimeStamp = message.TimeStamp,
                Type = BonusTypes.OfferToPurchaseByLead.EventName,
                Data = new Dictionary<string, string>()
                {
                    {"Amount", response.Amount.ToString("G")},
                    {"StakedCampaignId", message.CampaignId.ToString("D")},
                    {"UnitLocationCode", message.UnitLocationCode},
                    {"ReferralId", message.ReferralId}
                }
            });

            return true;
        }
    }
}
