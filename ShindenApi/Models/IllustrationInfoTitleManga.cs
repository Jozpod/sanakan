using Sanakan.ShindenApi.Models.Enums;
using Sanakan.ShindenApi.Utilities;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class IllustrationInfoTitleManga : IllustrationInfoTitle
    {
        [JsonPropertyName("manga")]
        public MangaInfo? Manga { get; set; }

        [JsonPropertyName("type")]
        public MangaType Type { get; set; }

        [JsonPropertyName("title_status")]
        public MangaStatus Status { get; set; }

        public string CoverUrl => UrlHelpers.GetBigImageURL(CoverId);
    }
}
