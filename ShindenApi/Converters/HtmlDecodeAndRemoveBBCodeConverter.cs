using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;

namespace Sanakan.ShindenApi.Converters
{
    public class HtmlDecodeAndRemoveBBCodeConverter : JsonConverter<string>
    {
        private static readonly Regex _removeBBCodeRegex = new(@"\[(.*?)\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new Exception("Invalid value type");
            }

            var str = reader.GetString();

            return _removeBBCodeRegex.Replace(HttpUtility.HtmlDecode(str)!, string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
