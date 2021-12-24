using Sanakan.ShindenApi.Models.Enums;
using System.Collections.Generic;

namespace Sanakan.ShindenApi.Converters
{
    public class AnimeStatusConverter : EnumConverter<AnimeStatus>
    {
        private static IDictionary<string, AnimeStatus> _map = new Dictionary<string, AnimeStatus>
        {
            { "currently airing", AnimeStatus.CurrentlyAiring },
            { "finished airing", AnimeStatus.FinishedAiring },
            { "not yet aired", AnimeStatus.NotYetAired },
            { "proposal", AnimeStatus.Proposal },
        };

        public AnimeStatusConverter() : base(_map, AnimeStatus.NotSpecified) {}
    }
}