using Sanakan.ShindenApi.Models.Enums;
using System.Collections.Generic;

namespace Sanakan.ShindenApi.Converters
{
    public class MpaaRatingConverter : EnumConverter<MpaaRating>
    {
        private static IDictionary<string, MpaaRating> _map = new Dictionary<string, MpaaRating>
        {
            { "g", MpaaRating.G },
            { "r", MpaaRating.R },
            { "pg", MpaaRating.PG },
            { "rx", MpaaRating.Rx },
            { "ry", MpaaRating.Ry },
            { "r+", MpaaRating.RPLUS },
            { "pg-13", MpaaRating.PGThirteen },
        };

        public MpaaRatingConverter()
            : base(_map, MpaaRating.NotSpecified)
        {
        }
    }
}