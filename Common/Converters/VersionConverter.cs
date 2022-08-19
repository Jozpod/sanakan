using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sanakan.Common.Converters
{
    public class VersionConverter : JsonConverter<Version>
    {
        public override Version Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new Exception("Invalid format for Version property");
            }

            return new(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
