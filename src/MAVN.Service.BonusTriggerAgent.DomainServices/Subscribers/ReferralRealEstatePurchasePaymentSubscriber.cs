using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.BonusTriggerAgent.Contract.Events;
using MAVN.Service.BonusTriggerAgent.Domain;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.CurrencyConvertor.Client.Models.Enums;
using Lykke.Service.RealEstateBonusAgent.Contract.Events;

namespace MAVN.Service.BonusTriggerAgent.DomainServices.Subscribers
{
    public class ReferralRealEstatePurchasePaymentSubscriber : RabbitSubscriber<ReferralRealEstatePurchasePaymentEvent>
    {
        private readonly string _assetName;
        private readonly IRabbitPublisher<BonusTriggerEvent> _bonusTriggerEventPublisher;
        private readonly ICurrencyConvertorClient _currencyConverterClient;

        public ReferralRealEstatePurchasePaymentSubscriber(
            string connectionString,
            string exchangeName,
            string assetName,
            ILogFactory logFactory,
            IRabbitPublisher<BonusTriggerEvent> bonusTriggerEventPublisher,
            ICurrencyConvertorClient currencyConverterClient)
            : base(connectionString, exchangeName, logFactory)
        {
            _bonusTriggerEventPublisher = bonusTriggerEventPublisher ??
                throw new ArgumentNullException(nameof(bonusTriggerEventPublisher));
            _currencyConverterClient = currencyConverterClient ??
                                       throw new ArgumentNullException(nameof(currencyConverterClient));
            _assetName = assetName;

            GuidsFieldsToValidate.Add(nameof(ReferralRealEstatePurchasePaymentEvent.CustomerId));
            GuidsFieldsToValidate.Add(nameof(ReferralRealEstatePurchasePaymentEvent.ReferralId));
        }

        public override async Task<bool> ProcessMessageAsync(ReferralRealEstatePurchasePaymentEvent message)
        {
            if (string.IsNullOrWhiteSpace(message.CurrencyCode))
            {
                Log.Error(null, "Currency is missing in Referral Property First Purchase Event", context: message, process: nameof(ProcessMessageAsync));
                return false;
            }

            var amount = message.NetPropertyAmount;

            if (amount < 0)
            {
                Log.Error(null, "NetPropertyAmount has invalid value in Referral Property First Purchase Event", context: message, process: nameof(ProcessMessageAsync));
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

            var bonusTriggerEvent = new BonusTriggerEvent
            {
                CustomerId = message.CustomerId,
                TimeStamp = DateTime.UtcNow,
                Type = BonusTypes.ReferralPropertyFirstPurchase.EventName,
                Data = new Dictionary<string, string>()
                {
                    {"Amount", response.Amount.ToString("G")},
                    {"IsDownPayment", message.IsDownPayment.ToString()},
                    {"PurchaseCompletionPercentage", message.PurchaseCompletionPercentage.ToString() },
                    {"PaymentId", message.ReferralId },
                    {"StakedCampaignId", message.CampaignId.ToString("D")},
                    {"UnitLocationCode", message.UnitLocationCode}
                }
            };

            await _bonusTriggerEventPublisher.PublishAsync(bonusTriggerEvent);

            return true;
        }
    }
}
