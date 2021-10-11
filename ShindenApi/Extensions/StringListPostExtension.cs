using System.Collections.Generic;
using System.Linq;

namespace Shinden.Extensions
{
    public static class StringListPostExtension
    {
        public static void AppendPOST(this List<string> list, string name, string value)
        {
            if (value != null && name != null) list.Add($"{name}={value}");
        }

        public static void AppendPOST(this List<string> list, string name, string[] values)
        {
            if (values == null || name == null) return;
            if (values.Length <= 0) return;

            list.AddRange(values.Select((t, i) => $"{name}[{i}]={t}"));
        }

        public static void AppendPOST(this List<string> list, string name, bool? value)
        {
            if (value.HasValue && name != null) list.Add($"{name}={value.Value.ToString().ToLower()}");
        }

        public static void AppendPOSTAsInt(this List<string> list, string name, bool? value)
        {
            if (value.HasValue && name != null) list.AppendPOST(name, value.Value ? 1 : 0);
        }

        public static void AppendPOST(this List<string> list, string name, long? value)
        {
            if (value.HasValue && name != null) list.Add($"{name}={value.Value}");
        }

        public static void AppendPOST(this List<string> list, string name, char? value)
        {
            if (value.HasValue && name != null) list.Add($"{name}={value.Value}");
        }
    }
}