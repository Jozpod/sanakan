using System;

namespace Sanakan.Common
{
    /// <summary>
    /// Abstracts access to system clock.
    /// </summary>
    public interface ISystemClock
    {
        /// <summary>
        /// <see cref="DateTime.UtcNow"/>.
        /// </summary>
        DateTime UtcNow { get; }

        DateTime StartOfMonth { get; }
    }
}
