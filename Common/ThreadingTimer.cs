using System;
using System.Threading;

namespace Sanakan.Common
{
    internal class ThreadingTimer : ITimer, IDisposable
    {
        private Timer? _timer;

        public event TimerEventHandler? Tick;

        public void Start(TimeSpan dueTime, TimeSpan period, object state)
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            _timer = new Timer(TimerCallback, state, dueTime, period);
        }

        public void Start(TimeSpan dueTime, TimeSpan period)
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            _timer = new Timer(TimerCallback, null, dueTime, period);
        }

        public void Dispose()
        {
            if(_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        public void Stop()
        {
            if(_timer == null)
            {
                return;
            }

            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void TimerCallback(object? state)
        {
            Tick?.Invoke(this, new TimerEventArgs(state));
        }
    }
}
