using Sanakan.ShindenApi.Models.Enums;
using Sanakan.ShindenApi.Utilities;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class IllustrationInfoTitleAnime : IllustrationInfoTitle
    {
        [JsonPropertyName("anime")]
        public AnimeInfo? Anime { get; set; }

        public AnimeType Type => Anime?.AnimeType ?? AnimeType.NotSpecified;

        public string AnimeUrl => UrlHelpers.GetSeriesURL(TitleId);
    }
}
