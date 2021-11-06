using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Converters
{
    public class EnumConverter<T> : JsonConverter<T>
    {
        private readonly IDictionary<string, T> _dictionary;

        public EnumConverter(IDictionary<string, T> dictionary)
        {
            _dictionary = dictionary;
        }

        public override T Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new Exception("");
            }

            var enumString = reader.GetString();

            if(_dictionary.TryGetValue(enumString, out var enumValue))
            {
                return enumValue;
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
