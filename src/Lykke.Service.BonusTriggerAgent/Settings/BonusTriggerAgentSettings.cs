using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.BonusTriggerAgent.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class BonusTriggerAgentSettings
    {
        public DbSettings Db { get; set; }

        public RabbitMqSettings RabbitMq { get; set; }

        public string BaseCurrencyCode { get; set; }

        [Optional]
        public bool? IsRealEstateFeatureDisabled { get; set; }
    }
}
