using Sanakan.ShindenApi.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Converters
{
    public class IllustrationConverter : JsonConverter<IllustrationInfoTitle>
    {
        public override IllustrationInfoTitle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            using var jsonDocument = JsonDocument.ParseValue(ref reader);

            if (!jsonDocument.RootElement.TryGetProperty("type", out var typeProperty))
            {
                throw new JsonException();
            }

            var type = typeProperty.GetString();

            if (type == null)
            {
                throw new JsonException();
            }

            IllustrationInfoTitle result;
            var jsonObject = jsonDocument.RootElement.GetRawText();

            if (type.ToLower().Equals("anime"))
            {
                result = JsonSerializer.Deserialize<IllustrationInfoTitleAnime>(jsonObject, options)!;
            }
            else
            {
                result = JsonSerializer.Deserialize<IllustrationInfoTitleManga>(jsonObject, options)!;
            }

            return result;
        }

        public override void Write(Utf8JsonWriter writer, IllustrationInfoTitle value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
