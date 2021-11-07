using Sanakan.ShindenApi.Models.Enums;
using System.Collections.Generic;

namespace Sanakan.ShindenApi.Converters
{
    public class AlternativeTitleTypeConverter : EnumConverter<AlternativeTitleType>
    {
        private static IDictionary<string, AlternativeTitleType> _map = new Dictionary<string, AlternativeTitleType>
        {
            { "official", AlternativeTitleType.Official },
            { "translated", AlternativeTitleType.Translated },
            { "alternative", AlternativeTitleType.Alternative },
        };

        public AlternativeTitleTypeConverter() : base(_map, AlternativeTitleType.NotSpecified) {}
    }
}