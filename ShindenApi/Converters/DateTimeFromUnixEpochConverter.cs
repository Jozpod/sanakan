using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Converters
{
    public class DateTimeFromUnixEpochConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new Exception("Invalid value type");
            }

            var value = reader.GetDouble();

            return DateTime.UnixEpoch.AddSeconds(value);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
