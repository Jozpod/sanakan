using Sanakan.ShindenApi.Models.Enums;
using System.Collections.Generic;

namespace Sanakan.ShindenApi.Converters
{
    public class EpisodeTypeConverter : EnumConverter<EpisodeType>
    {
        private static IDictionary<string, EpisodeType> _map = new Dictionary<string, EpisodeType>
        {
            { "ova", EpisodeType.Ova },
            { "special", EpisodeType.Special },
            { "standard", EpisodeType.Standard },
        };

        public EpisodeTypeConverter() : base(_map, EpisodeType.NotSpecified) {}
    }
}