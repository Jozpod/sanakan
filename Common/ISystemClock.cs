using System;

namespace Sanakan.Common
{
    public interface ISystemClock
    {
        DateTime UtcNow { get; }
    }
}
