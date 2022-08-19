using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace Sanakan.ShindenApi.Converters
{
    public class HtmlDecodeConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new Exception("Invalid value type");
            }

            var str = reader.GetString();

            return HttpUtility.HtmlDecode(str);
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
