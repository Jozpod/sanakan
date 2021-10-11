using Shinden.Models;
using System.Collections.Generic;

namespace Shinden.Extensions
{
    public static class FavouriteConfigExtension
    {
        public static string Build(this FavouriteConfig config)
        {
            var list = new List<string>();
            list.AppendPOST("id", config.Type.ToQuery(config.FavouriteId));
            return string.Join("&", list);
        }
    }
}
