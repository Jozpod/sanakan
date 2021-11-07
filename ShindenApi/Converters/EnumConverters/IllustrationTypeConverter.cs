using Sanakan.ShindenApi.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Converters
{
    public class IllustrationTypeConverter : JsonConverter<IllustrationType>
    {
        public override IllustrationType Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new Exception("Invalid value type");
            }

            var value = reader.GetString();

            return value.ToLower().Equals("anime") ? IllustrationType.Anime : IllustrationType.Manga;
        }

        public override void Write(Utf8JsonWriter writer, IllustrationType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}