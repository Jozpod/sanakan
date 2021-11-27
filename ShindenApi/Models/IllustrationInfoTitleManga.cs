using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models.Enums;
using Sanakan.ShindenApi.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class IllustrationInfoTitleManga : IllustrationInfoTitle
    {
        [JsonPropertyName("manga")]
        public MangaInfo? Manga { get; set; }

        [JsonPropertyName("type")]
        public MangaType Type { get; set; }

        public string CoverUrl => UrlHelpers.GetBigImageURL(CoverId);
    }
}
