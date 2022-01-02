using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Converters
{
    public class TimeSpanFromMinutesConverter : JsonConverter<TimeSpan?>
    {
        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new Exception("Invalid value type");
            }

            var value = reader.GetInt32();

            return TimeSpan.FromMinutes(value);
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
