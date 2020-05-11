using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.BonusTriggerAgent.Contract.Events;
using MAVN.Service.BonusTriggerAgent.Domain;
using MAVN.Service.CurrencyConvertor.Client;
using MAVN.Service.CurrencyConvertor.Client.Models.Enums;
using MAVN.Service.Referral.Contract.Events;

namespace MAVN.Service.BonusTriggerAgent.DomainServices.Subscribers
{
    public class HotelCheckoutReferralSubscriber : RabbitSubscriber<HotelReferralUsedEvent>
    {
        private readonly string _assetName;
        private readonly IRabbitPublisher<BonusTriggerEvent> _bonusTriggerEventPublisher;
        private readonly ICurrencyConvertorClient _currencyConverterClient;
        private readonly ILog _log;

        public HotelCheckoutReferralSubscriber(
            string connectionString,
            string exchangeName,
            string assetName,
            IRabbitPublisher<BonusTriggerEvent> bonusTriggerEventPublisher,
            ICurrencyConvertorClient currencyConverterClient,
            ILogFactory logFactory)
            : base(connectionString, exchangeName, logFactory)
        {
            _assetName = assetName;
            _bonusTriggerEventPublisher = bonusTriggerEventPublisher;
            _currencyConverterClient = currencyConverterClient;
            _log = logFactory.CreateLog(this);
            GuidsFieldsToValidate.Add(nameof(HotelReferralUsedEvent.CustomerId));
        }

        public override async Task<bool> ProcessMessageAsync(HotelReferralUsedEvent message)
        {
            var response = await _currencyConverterClient.Converter
                .ConvertAsync(message.CurrencyCode, _assetName, message.Amount);

            if (response.ErrorCode != ConverterErrorCode.None)
            {
                _log.Error(message: "An error occured while converting currency amount",
                    context: $"from: {message.CurrencyCode}; to: {_assetName}; error: {response.ErrorCode}");

                return false;
            }

            var bonusTriggerEvent = new BonusTriggerEvent
            {
                CustomerId = message.CustomerId,
                TimeStamp = DateTime.UtcNow,
                Type = BonusTypes.HotelCheckoutReferral.EventName,
                PartnerId = message.PartnerId,
                LocationId = message.LocationId,
                Data = new Dictionary<string, string>
                {
                    {"Amount", response.Amount.ToString("G")},
                    {"ReferralId", message.ReferralId}
                }
            };

            if (message.StakedCampaignId.HasValue)
            {
                bonusTriggerEvent.Data.Add("StakedCampaignId", message.StakedCampaignId.Value.ToString());
            }

            await _bonusTriggerEventPublisher.PublishAsync(bonusTriggerEvent);

            _log.Info("Hotel stay referral bonus event published", context: $"CustomerId: {message.CustomerId}");

            return true;
        }
    }
}
