using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sanakan.Common.Converters
{
    public class TimeZoneInfoConverter : JsonConverter<TimeZoneInfo>
    {
        public override TimeZoneInfo Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new Exception("Invalid format for Version property");
            }

            return TimeZoneInfo.FindSystemTimeZoneById(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, TimeZoneInfo value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.StandardName);
        }
    }
}