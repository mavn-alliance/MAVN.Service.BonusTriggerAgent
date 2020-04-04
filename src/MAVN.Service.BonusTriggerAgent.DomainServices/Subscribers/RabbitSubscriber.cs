using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;

namespace MAVN.Service.BonusTriggerAgent.DomainServices.Subscribers
{
    public abstract class RabbitSubscriber<TMessage> : IStartStop
    {
        private readonly ILogFactory _logFactory;
        private readonly string _connectionString;
        private readonly string _exchangeName;
        private readonly string _contextName;

        private RabbitMqSubscriber<TMessage> _subscriber;

        protected readonly ILog Log;
        protected IList<string> GuidsFieldsToValidate { get; set; } = new List<string>();

        protected RabbitSubscriber(string connectionString, string exchangeName, ILogFactory logFactory)
        {
            Log = logFactory.CreateLog(this);
            _logFactory = logFactory;

            _connectionString = connectionString;
            _exchangeName = exchangeName;

            _contextName = GetType().Name;
        }

        public void Start()
        {
            var rabbitMqSubscriptionSettings = RabbitMqSubscriptionSettings.ForSubscriber(_connectionString,
                    _exchangeName,
                    "bonustriggeragent")
                .MakeDurable();

            _subscriber = new RabbitMqSubscriber<TMessage>(
                    _logFactory,
                    rabbitMqSubscriptionSettings,
                    new ResilientErrorHandlingStrategy(
                        _logFactory,
                        rabbitMqSubscriptionSettings,
                        TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<TMessage>())
                .Subscribe(StartProcessingAsync)
                .CreateDefaultBinding()
                .Start();
        }

        public void Stop()
        {
            _subscriber.Stop();
        }

        public void Dispose()
        {
            _subscriber.Dispose();
        }

        public async Task StartProcessingAsync(TMessage message)
        {
            Log.Info($"{_contextName} event received", message, process: nameof(StartProcessingAsync));

            if (!ValidateIdentifiers(message))
            {
                return;
            }

            if (!await ProcessMessageAsync(message))
            {
                Log.Error(null, $"{_contextName} event was not processed", message, process: nameof(StartProcessingAsync));
            }

            Log.Info($"{_contextName} event was processed", message, process: nameof(StartProcessingAsync));
        }

        private bool ValidateIdentifiers(TMessage message)
        {
            var messageType = typeof(TMessage);
            var isMessageValid = true;

            foreach (var fieldName in GuidsFieldsToValidate)
            {
                var propertyInfo = messageType.GetProperty(fieldName);

                var fieldValue = (string)propertyInfo.GetValue(message, null);

                if (!Guid.TryParse(fieldValue, out _))
                {
                    Log.Error(null, $"{fieldName} has invalid format in {nameof(message)}", message, process: nameof(ValidateIdentifiers));
                    isMessageValid = false;
                }
            }

            return isMessageValid;
        }

        public abstract Task<bool> ProcessMessageAsync(TMessage message);
    }
}
