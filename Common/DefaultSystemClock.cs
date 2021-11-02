using System;

namespace Sanakan.Common
{
    internal class DefaultSystemClock : ISystemClock
    {
        public DateTime StartOfMonth
        {
            get
            {
                var currentDate = DateTime.UtcNow.Date;
                return currentDate.AddDays(1 - currentDate.Day);
            }
        }

        DateTime ISystemClock.UtcNow => DateTime.UtcNow;
    }
}
