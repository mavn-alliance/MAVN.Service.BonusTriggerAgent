using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lykke.Common;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.BonusTriggerAgent.Contract.Events;
using MAVN.Service.BonusTriggerAgent.Domain;

namespace MAVN.Service.BonusTriggerAgent.DomainServices
{
    public class BonusTypePublisher : IStartStop
    {
        private readonly IRabbitPublisher<BonusTypeDetectedEvent> _rabbitPublisher;

        public BonusTypePublisher(
            IRabbitPublisher<BonusTypeDetectedEvent> rabbitPublisher)
        {
            _rabbitPublisher = rabbitPublisher;
        }

        public void Start()
        {
            foreach (var bonusType in GetBonusTypes())
            {
                _rabbitPublisher.PublishAsync(new BonusTypeDetectedEvent
                {
                    DisplayName = bonusType.DisplayName,
                    EventName = bonusType.EventName,
                    Vertical = bonusType.Vertical,
                    AllowInfinite = bonusType.AllowInfinite,
                    AllowPercentage = bonusType.AllowPercentage,
                    AllowConversionRate = bonusType.AllowConversionRate, 
                    IsEnabled = bonusType.IsEnabled,
                    IsStakeable = bonusType.IsStakeable,
                    IsHidden = bonusType.IsHidden,
                    Order = bonusType.Order,
                    RewardHasRatio = bonusType.RewardHasRatio
                });
            }
        }

        private static IEnumerable<BonusType> GetBonusTypes()
        {
            return typeof(BonusTypes)
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Select(member => (BonusType) member.GetValue(null));
        }

        public void Dispose()
        {
            Stop();
        }

        public void Stop()
        {
        }
    }
}
