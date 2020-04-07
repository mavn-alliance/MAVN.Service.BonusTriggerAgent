//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Lykke.Common.Log;
//using Lykke.RabbitMqBroker.Publisher;
//using MAVN.Service.BonusTriggerAgent.Contract.Events;
//using MAVN.Service.BonusTriggerAgent.Domain;
//using Lykke.Service.MAVNPropertyIntegration.Contract.MAVNEvents;

//namespace MAVN.Service.BonusTriggerAgent.DomainServices.Subscribers
//{
//    public class PropertyPurchaseFirstRewardSubscriber
//        : RabbitSubscriber<MAVNPropertyPurchaseFirstRewardEvent>
//    {
//        private readonly IRabbitPublisher<BonusTriggerEvent> _bonusTriggerEventPublisher;

//        public PropertyPurchaseFirstRewardSubscriber(
//            string connectionString,
//            string exchangeName,
//            IRabbitPublisher<BonusTriggerEvent> bonusTriggerEventPublisher,
//            ILogFactory logFactory)
//            : base(connectionString, exchangeName, logFactory)
//        {
//            _bonusTriggerEventPublisher = bonusTriggerEventPublisher
//                ?? throw new ArgumentNullException(nameof(bonusTriggerEventPublisher));
//        }

//        public override async Task<bool> ProcessMessageAsync(MAVNPropertyPurchaseFirstRewardEvent message)
//        {
//            // Having customer id null here is valid case
//            if (string.IsNullOrEmpty(message.BuyerCustomerId))
//                return true;

//            var amount = message.NetPropertyPrice ?? 0M;

//            if (amount < 0)
//            {
//                Log.Error(null, "Amount has invalid value in Purchase Referral Event", context: message, process: nameof(ProcessMessageAsync));
//                return false;
//            }

//            await _bonusTriggerEventPublisher.PublishAsync(new BonusTriggerEvent
//            {
//                CustomerId = message.BuyerCustomerId,
//                TimeStamp = message.Timestamp,
//                Type = BonusTypes.PropertyPurchaseFirstReward.EventName,
//                Data = new Dictionary<string, string>()
//                {
//                    {"Amount", amount.ToString("G")},
//                    {"UnitLocationCode", message.UnitLocationCode}
//                }
//            });

//            return true;
//        }
//    }
//}
