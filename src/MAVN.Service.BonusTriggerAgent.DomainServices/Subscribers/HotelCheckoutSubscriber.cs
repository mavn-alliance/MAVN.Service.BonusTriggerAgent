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
using MAVN.Service.PartnersIntegration.Contract;

namespace MAVN.Service.BonusTriggerAgent.DomainServices.Subscribers
{
    public class HotelCheckoutSubscriber : RabbitSubscriber<BonusCustomerTriggerEvent>
    {
        private readonly string _assetName;
        private readonly IRabbitPublisher<BonusTriggerEvent> _bonusTriggerEventPublisher;
        private readonly ICurrencyConvertorClient _currencyConverterClient;
        private readonly ILog _log;

        public HotelCheckoutSubscriber(
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
            GuidsFieldsToValidate.Add(nameof(BonusCustomerTriggerEvent.CustomerId));
        }

        public override async Task<bool> ProcessMessageAsync(BonusCustomerTriggerEvent message)
        {
            var response = await _currencyConverterClient.Converter
                .ConvertAsync(message.Currency, _assetName, message.Amount);

            if (response.ErrorCode != ConverterErrorCode.None)
            {
                _log.Error(message: "An error occured while converting currency amount",
                    context: $"from: {message.Currency}; to: {_assetName}; error: {response.ErrorCode}");

                return false;
            }

            await _bonusTriggerEventPublisher.PublishAsync(new BonusTriggerEvent
            {
                CustomerId = message.CustomerId,
                TimeStamp = DateTime.UtcNow,
                Type = BonusTypes.HotelCheckout.EventName,
                PartnerId = message.PartnerId,
                LocationId = message.LocationId,
                Data = new Dictionary<string, string> {{"Amount", response.Amount.ToString("G")}}
            });

            _log.Info("Hotel stay bonus event published", context: $"CustomerId: {message.CustomerId}");

            return true;
        }
    }
}
