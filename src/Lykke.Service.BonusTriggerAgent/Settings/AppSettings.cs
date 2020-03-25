using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.CurrencyConvertor.Client;
using Lykke.Service.CustomerProfile.Client;

namespace Lykke.Service.BonusTriggerAgent.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public BonusTriggerAgentSettings BonusTriggerAgentService { get; set; }

        public CurrencyConvertorServiceClientSettings CurrencyConvertorServiceClient { get; set; }

        public CustomerProfileServiceClientSettings CustomerProfileServiceClient { get; set; }
    }
}
