using System;

namespace Sanakan.Common
{
    public interface ITimer
    {
        void Start(TimeSpan dueTime, TimeSpan period);
        void Stop();
        event TimerEventHandler Tick;
    }
}
