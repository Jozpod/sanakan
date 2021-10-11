using System.Collections.Generic;

namespace Shinden.Extensions
{
    public static class StringListJsonExtension
    {
        public static void AppendJSON(this List<string> list, string name, string value)
        {
            if (value != null && name != null) list.Add($"\"{name}\":\"{value}\"");
        }

        public static void AppendJSON(this List<string> list, string name, string[] values)
        {
            if (values == null || name == null) return;
            if (values.Length <= 0) return;

            for (var i = 0; i < values.Length; i++) values[i] = $"\"{values[i]}\"";
            list.Add($"\"{name}\":[" + string.Join(",", values) + "]");
        }

        public static void AppendJSON(this List<string> list, string name, bool? value)
        {
            if (value.HasValue && name != null) list.Add($"\"{name}\":{value.Value.ToString().ToLower()}");
        }

        public static void AppendJSONAsInt(this List<string> list, string name, bool? value)
        {
            if (value.HasValue && name != null) list.AppendJSON(name, value.Value ? 1 : 0);
        }

        public static void AppendJSON(this List<string> list, string name, long? value)
        {
            if (value.HasValue && name != null) list.Add($"\"{name}\":{value.Value}");
        }

        public static void AppendJSON(this List<string> list, string name, char? value)
        {
            if (value.HasValue && name != null) list.Add($"\"{name}\":\"{value.Value}\"");
        }
    }
}