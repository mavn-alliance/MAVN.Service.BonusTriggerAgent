using System;
using System.Collections.Generic;

namespace Lykke.Service.BonusTriggerAgent.Contract.Events
{
    /// <summary>
    /// Represents event that notify about new trigger for Bonus Engine
    /// </summary>
    public class BonusTriggerEvent
    {
        /// <summary>
        /// Represents Falcon's CustomerId
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Represents the Partner Id, can be null or empty
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// Represents the Location Id, can be null or empty
        /// </summary>
        public string LocationId { get; set; }

        /// <summary>
        /// Represents the type of the trigger event
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Represents the timeStamp when the event is triggered
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Represents a dictionary that contains additional trigger data
        /// </summary>
        public Dictionary<string, string> Data { get; set; }
    }
}
