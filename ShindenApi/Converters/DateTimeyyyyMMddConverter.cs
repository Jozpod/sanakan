using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Converters
{
    public class DateTimeyyyyMMddConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new Exception("Invalid value type");
            }

            var dateString = reader.GetString();

            if (string.IsNullOrEmpty(dateString))
            {
                return null;
            }

            return DateTime.ParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
