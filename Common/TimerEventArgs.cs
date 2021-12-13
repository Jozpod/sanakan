using System;

namespace Sanakan.Common
{
    public delegate void TimerEventHandler(object sender, TimerEventArgs e);

    public class TimerEventArgs : EventArgs
    {
        public TimerEventArgs(object? state)
        {
            State = state!;
        }

        public object State { get; private set; } = null;
    }
}
