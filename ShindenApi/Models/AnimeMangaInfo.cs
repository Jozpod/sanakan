using Sanakan.ShindenApi.API.Common;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class AnimeMangaInfo
    {
        [JsonPropertyName("title")]
        public TitleEntry Title { get; set; }
    }
}
