using System.Collections.Generic;

namespace Shinden.Extensions
{
    public static class RateMangaConfigExtension
    {
        public static string Build(this Models.RateMangaConfig rate)
        {
            var list = new List<string>();
            list.AppendPOST("type", rate.RateType.ToQuery());
            list.AppendPOST("value", rate.RateValue);
            return string.Join("&", list);
        }
    }
}