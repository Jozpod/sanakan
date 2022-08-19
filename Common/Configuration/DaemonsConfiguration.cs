using System;

namespace Sanakan.Common.Configuration
{
    public class DaemonsConfiguration
    {
        /// <summary>
        /// The inital amount of time to delay before capturing memory usage.
        /// </summary>
        public TimeSpan CaptureMemoryUsageDueTime { get; set; }

        /// <summary>
        /// The time interval between invocations of memory usage.
        /// </summary>
        public TimeSpan CaptureMemoryUsagePeriod { get; set; }

        /// <summary>
        /// The inital amount of time to delay before time status check.
        /// </summary>
        public TimeSpan ProfilePeriod { get; set; }

        /// <summary>
        /// The time interval between invocations of time status check.
        /// </summary>
        public TimeSpan ProfileDueTime { get; set; }

        /// <summary>
        /// The inital amount of time to delay before chaos mode reset.
        /// </summary>
        public TimeSpan ChaosDueTime { get; set; }

        /// <summary>
        /// The time interval between invocations of chaos mode reset.
        /// </summary>
        public TimeSpan ChaosPeriod { get; set; }

        /// <summary>
        ///  The inital amount of time to delay before session expiry check.
        /// </summary>
        public TimeSpan SessionDueTime { get; set; }

        /// <summary>
        /// The time interval between invocations of session expiry check.
        /// </summary>
        public TimeSpan SessionPeriod { get; set; }

        /// <summary>
        /// The inital amount of time to delay before penalty expiry check.
        /// </summary>
        public TimeSpan ModeratorPeriod { get; set; }

        /// <summary>
        /// The time interval between invocations of penalty expiry check.
        /// </summary>
        public TimeSpan ModeratorDueTime { get; set; }

        /// <summary>
        /// The inital amount of time to delay before supervisor subject reset.
        /// </summary>
        public TimeSpan SupervisorDueTime { get; set; }

        /// <summary>
        /// The time interval between invocations of supervisor subject reset.
        /// </summary>
        public TimeSpan SupervisorPeriod { get; set; }
    }
}
