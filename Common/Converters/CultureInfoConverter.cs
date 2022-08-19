using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sanakan.Common.Converters
{
    public class CultureInfoConverter : JsonConverter<CultureInfo>
    {
        public override CultureInfo Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new Exception("Invalid format for Version property");
            }

            return new(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Name);
        }
    }
}
