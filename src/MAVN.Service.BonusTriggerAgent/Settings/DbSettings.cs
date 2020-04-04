using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.BonusTriggerAgent.Settings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
