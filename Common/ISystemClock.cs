using System;

namespace Common
{
    public interface ISystemClock
    {
        DateTime UtcNow { get; }
    }
}
