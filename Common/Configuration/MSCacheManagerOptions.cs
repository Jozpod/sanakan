using System;

namespace Sanakan.Common.Configuration
{
    public class MSCacheManagerOptions
    {
        public TimeSpan SlidingExpiration { get; set; }
        public TimeSpan AbsoluteExpirationRelativeToNow { get; set; }
    }
}
