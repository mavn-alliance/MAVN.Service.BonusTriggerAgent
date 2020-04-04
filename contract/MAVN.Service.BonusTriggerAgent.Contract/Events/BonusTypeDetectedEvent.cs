using Lykke.Service.PartnerManagement.Client.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MAVN.Service.BonusTriggerAgent.Contract.Events
{
    /// <summary>
    /// Represents Bonus Type detection
    /// </summary>
    public class BonusTypeDetectedEvent
    {
        /// <summary>
        /// Display name of the Bonus Type
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Event name used in database
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Vertical to which Bonus Type belongs
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Vertical? Vertical { get; set; }

        /// <summary>
        /// Does the Bonus Type allow infinite repetitions
        /// </summary>
        public bool AllowInfinite { get; set; }

        /// <summary>
        /// Does the Bonus Type allow percentage
        /// </summary>
        public bool AllowPercentage { get; set; }

        /// <summary>
        /// Does the Bonus Type allow conversion rate
        /// </summary>
        public bool AllowConversionRate { get; set; }

        /// <summary>
        /// Is the Bonus Type enabled
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Is the bonus type stakeable
        /// </summary>
        public bool IsStakeable { get; set; }

        /// <summary>
        /// Identifies is the type should be hidden
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Represents bonus type order number
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Indicates if the bonus type has ratio
        /// </summary>
        public bool RewardHasRatio { get; set; }
    }
}
