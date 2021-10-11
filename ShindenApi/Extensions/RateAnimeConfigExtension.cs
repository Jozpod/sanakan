using System.Collections.Generic;

namespace Shinden.Extensions
{
    public static class RateAnimeConfigExtension
    {
        public static string Build(this Models.RateAnimeConfig rate)
        {
            var list = new List<string>();
            list.AppendPOST("type", rate.RateType.ToQuery());
            list.AppendPOST("value", rate.RateValue);
            return string.Join("&", list);
        }
    }
}