using System;

namespace Sanakan.DAL.Models.Analytics
{

    public class SystemAnalytics
    {
        public ulong Id { get; set; }
        public long Value { get; set; }
        public DateTime MeasuredOn { get; set; }
        public SystemAnalyticsEventType Type { get; set; }
    }
}