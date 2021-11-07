using Sanakan.ShindenApi.Models.Enums;
using System.Collections.Generic;

namespace Sanakan.ShindenApi.Converters
{
    public class MangaTypeConverter : EnumConverter<MangaType>
    {
        private static IDictionary<string, MangaType> _map = new Dictionary<string, MangaType>
        {
            { "light_novel", MangaType.LightNovel },
            { "doujinshi", MangaType.Doujinshi },
            { "novel", MangaType.LightNovel },
            { "one_shot", MangaType.OneShot },
            { "one shot", MangaType.OneShot },
            { "doujin", MangaType.Doujinshi },
            { "manhua", MangaType.Manhua },
            { "manhwa", MangaType.Manhwa },
            { "manga", MangaType.Manga },
        };

        public MangaTypeConverter() : base(_map, MangaType.NotSpecified) {}
    }
}