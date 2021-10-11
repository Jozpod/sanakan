using System.Collections.Generic;
using Shinden.Models;

namespace Shinden.Extensions
{
    public static class ChangeTitleStatusConfigExtension
    {
        public static string Build(this ChangeTitleStatusConfig config)
        {
            var list = new List<string>();
            list.AppendPOST("status", config.NewListType.ToQuery());
            return string.Join("&", list);
        }
    }
}