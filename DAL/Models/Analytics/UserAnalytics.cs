using System;

namespace Sanakan.DAL.Models.Analytics
{
    public class UserAnalytics
    {
        public ulong Id { get; set; }
        public long Value { get; set; }
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public DateTime MeasureDate { get; set; }
        public UserAnalyticsEventType Type { get; set; }
    }
}