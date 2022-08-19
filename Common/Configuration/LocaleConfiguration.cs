using System;
using System.Globalization;

namespace Sanakan.Common.Configuration
{
    /// <summary>
    /// Describes locale.
    /// </summary>
    public class LocaleConfiguration
    {
        /// <summary>
        /// The culture information which bot will use.
        /// </summary>
        public CultureInfo Language { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// The time zone which bot will use.
        /// </summary>
        public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;
    }
}
