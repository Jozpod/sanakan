using Sanakan.ShindenApi.Models.Enums;
using System.Collections.Generic;

namespace Sanakan.ShindenApi.Converters
{
    public class AnimeTypeConverter : EnumConverter<AnimeType>
    {
        private static IDictionary<string, AnimeType> _map = new Dictionary<string, AnimeType>
        {
            { "tv", AnimeType.Tv },
            { "ona", AnimeType.Ona },
            { "ova", AnimeType.Ova },
            { "movie", AnimeType.Movie },
            { "music", AnimeType.Music },
            { "special", AnimeType.Special },
        };

        public AnimeTypeConverter()
            : base(_map, AnimeType.NotSpecified)
        {
        }
    }
}