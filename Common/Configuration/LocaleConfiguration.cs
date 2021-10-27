using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Common.Configuration
{
    public class LocaleConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public CultureInfo Language { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TimeZone TimeZone { get; set; }
    }
}
