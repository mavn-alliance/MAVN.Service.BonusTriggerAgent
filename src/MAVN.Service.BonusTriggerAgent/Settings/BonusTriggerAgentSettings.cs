using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.BonusTriggerAgent.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class BonusTriggerAgentSettings
    {
        public DbSettings Db { get; set; }

        public RabbitMqSettings RabbitMq { get; set; }

        public string BaseCurrencyCode { get; set; }

        [Optional]
        public bool? IsRealEstateFeatureDisabled { get; set; }

        [Optional]
        public bool IsPhoneVerificationDisabled { get; set; }
    }
}
