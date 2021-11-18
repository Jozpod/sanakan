using System;

namespace Sanakan.Common
{
    public interface ITimer
    {
        /// <summary>
        /// Starts timer.
        /// </summary>
        /// <param name="dueTime">The amount of time to delay before the callback is invoked.</param>
        /// <param name="period">The time interval between invocations of callback</param>
        /// <param name="state">An object containing information to be used by the callback method, or null.</param>
        void Start(TimeSpan dueTime, TimeSpan period, object state);

        /// <summary>
        /// Starts timer.
        /// </summary>
        /// <param name="dueTime">The amount of time to delay before the callback is invoked.</param>
        /// <param name="period">The time interval between invocations of callback.</param>
        void Start(TimeSpan dueTime, TimeSpan period);
        
        /// <summary>
        /// Sets threading timer due time and period to infinity.
        /// </summary>
        void Stop();

        event TimerEventHandler Tick;
    }
}
