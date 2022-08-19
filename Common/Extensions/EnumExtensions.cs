using System;
using System.Collections.Generic;

namespace Sanakan.Common.Extensions
{
    public static class EnumExtensions
    {
        private static IDictionary<Type, object> _enumDict = new Dictionary<Type, object>();

        public static T Next<T>(this T src) where T : Enum
        {
            var type = src.GetType();
            T[] values;

            if(_enumDict.TryGetValue(type, out var objValues))
            {
                values = (T[])objValues;
            }
            else
            {
                values = (T[])Enum.GetValues(type);
                _enumDict[type] = values;
            }

            int j = Array.IndexOf<T>(values, src) + 1;

            return values.Length == j  ? values[0] : values[j];
        }
    }
}
