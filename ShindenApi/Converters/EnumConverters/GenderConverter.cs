using Sanakan.ShindenApi.Models.Enums;
using System.Collections.Generic;

namespace Sanakan.ShindenApi.Converters
{
    public class GenderConverter : EnumConverter<Gender>
    {
        private static IDictionary<string, Gender> _map = new Dictionary<string, Gender>
        {
            { "m", Gender.Male },
            { "k", Gender.Female },
            { "f", Gender.Female },
            { "male", Gender.Male },
            { "other", Gender.Other },
            { "female", Gender.Female },
        };

        public GenderConverter()
            : base(_map, Gender.NotSpecified)
        {
        }
    }
}