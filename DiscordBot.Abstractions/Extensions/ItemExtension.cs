using Sanakan.DAL.Models;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.Extensions
{
    public static class ItemExtension
    {

        private static readonly StringBuilder _stringBuilder = new (200);

        public static string ToItemList(this IEnumerable<Item> itemList)
        {
            _stringBuilder.Clear();

            var index = 0;
            foreach (var item in itemList)
            {
                _stringBuilder.AppendFormat("**[{0}]** {1} x{2}\n", index + 1, item.Name, item.Count);
                index++;
            }

            return _stringBuilder.ToString();
        }
    }
}
