﻿using Sanakan.Common;
using System;

namespace Web.Tests.HostedServices
{
    public class FakeTimer : ITimer
    {
        public bool Stopped { get; set; }

        public void Start(TimeSpan dueTime, TimeSpan period) { }
        public void Stop() => Stopped = true;
        public void Change(TimeSpan dueTime, TimeSpan period) { }

        public void RaiseTickEvent() => Tick.Invoke(this, new TimerEventArgs(null));

        public event TimerEventHandler Tick;
    }
}
