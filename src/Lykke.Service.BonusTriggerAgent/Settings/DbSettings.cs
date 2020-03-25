using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.BonusTriggerAgent.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
