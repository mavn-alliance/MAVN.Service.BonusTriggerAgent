using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using MAVN.Service.CurrencyConvertor.Client;
using MAVN.Service.CustomerProfile.Client;

namespace MAVN.Service.BonusTriggerAgent.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public BonusTriggerAgentSettings BonusTriggerAgentService { get; set; }

        public CurrencyConvertorServiceClientSettings CurrencyConvertorServiceClient { get; set; }

        public CustomerProfileServiceClientSettings CustomerProfileServiceClient { get; set; }
    }
}
