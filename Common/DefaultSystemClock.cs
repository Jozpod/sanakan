using System;

namespace Common
{
    public class DefaultSystemClock : ISystemClock
    {
        DateTime ISystemClock.UtcNow => DateTime.UtcNow;
    }
}
