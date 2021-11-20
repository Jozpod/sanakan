using System;

namespace Sanakan.Common.Configuration
{
    public class DaemonsConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan CaptureMemoryUsageDueTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan CaptureMemoryUsagePeriod { get; set; }
        public TimeSpan ProfilePeriod { get; set; }
        public TimeSpan ProfileDueTime { get; set; }
        public TimeSpan ChaosDueTime { get; set; }
        public TimeSpan ChaosPeriod { get; set; }
        public TimeSpan SessionDueTime { get; set; }
        public TimeSpan SessionPeriod { get; set; }
        public TimeSpan ModeratorPeriod { get; set; }
        public TimeSpan ModeratorDueTime { get; set; }
        public TimeSpan SupervisorDueTime { get; set; }
        public TimeSpan SupervisorPeriod { get; set; }
    }
}
