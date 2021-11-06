using System;
using System.Globalization;

namespace Sanakan.Common.Configuration
{
    public class LocaleConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public CultureInfo Language { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// 
        /// </summary>
        public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;
    }
}
