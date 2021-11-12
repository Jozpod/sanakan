using System;

namespace Sanakan.DAL.Models.Analytics
{
    public class UserAnalytics
    {
        public ulong Id { get; set; }

        /// <summary>
        /// The user level.
        /// </summary>
        public ulong Value { get; set; }

        /// <summary>
        /// The Discord user identifier.
        /// </summary>
        public ulong UserId { get; set; }

        /// <summary>
        /// The Discord guild (server) identifier.
        /// </summary>
        public ulong? GuildId { get; set; }

        /// <summary>
        /// The datetime when event was measured.
        /// </summary>
        public DateTime MeasuredOn { get; set; }

        /// <summary>
        /// The user analytics event type.
        /// </summary>
        public UserAnalyticsEventType Type { get; set; }
    }
}