﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Sanakan.Common
{
    public class ThreadingTimer : ITimer, IDisposable
    {
        private Timer _timer;

        public void Start(TimeSpan dueTime, TimeSpan period)
        {
            _timer = new Timer(TimerCallback, null, dueTime, period);
        }

        public void Dispose() => _timer.Dispose();

        private void TimerCallback(object? state)
        {
            Tick?.Invoke(this, new TimerEventArgs(state));
        }

        public void Stop() => _timer.Change(Timeout.Infinite, Timeout.Infinite);

        public event TimerEventHandler? Tick;
    }
}