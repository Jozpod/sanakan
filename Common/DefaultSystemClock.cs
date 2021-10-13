using System;

namespace Sanakan.Common
{
    public class DefaultSystemClock : ISystemClock
    {
        DateTime ISystemClock.UtcNow => DateTime.UtcNow;
    }
}
