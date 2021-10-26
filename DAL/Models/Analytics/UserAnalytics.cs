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
        public ulong GuildId { get; set; }
        public DateTime MeasureDate { get; set; }
        public UserAnalyticsEventType Type { get; set; }
    }
}