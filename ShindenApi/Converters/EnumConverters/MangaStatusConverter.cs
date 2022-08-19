using Sanakan.ShindenApi.Models.Enums;
using System.Collections.Generic;

namespace Sanakan.ShindenApi.Converters
{
    public class MangaStatusConverter : EnumConverter<MangaStatus>
    {
        private static IDictionary<string, MangaStatus> _map = new Dictionary<string, MangaStatus>
        {
            { "finished", MangaStatus.Finished },
            { "not yet published", MangaStatus.NotYetPublished },
            { "publishing", MangaStatus.Publishing },
        };

        public MangaStatusConverter()
            : base(_map, MangaStatus.NotSpecified)
        {
        }
    }
}