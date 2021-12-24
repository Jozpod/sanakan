using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.Extensions
{
    public static class ItemExtension
    {

        public static string ToItemList(this IEnumerable<Item> itemList)
        {
            var stringBuilder = new StringBuilder(200);

            var index = 0;
            foreach (var item in itemList)
            {
                stringBuilder.AppendFormat("**[{0}]** {1} x{2}\n", index + 1, item.Name, item.Count);
                index++;
            }

            return stringBuilder.ToString();
        }
    }
}
