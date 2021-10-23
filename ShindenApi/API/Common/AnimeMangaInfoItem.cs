using Newtonsoft.Json;

namespace Sanakan.ShindenApi.API.Common
{
    public class AnimeMangaInfoItem
    {
        [JsonProperty("tag_id")]
        public string TagId { get; set; }

        [JsonProperty("national_name")]
        public string NationalName { get; set; }

        [JsonProperty("is_accepted")]
        public string IsAccepted { get; set; }

        [JsonProperty("parent_id")]
        public string ParentId { get; set; }

        [JsonProperty("tag_type")]
        public string TagType { get; set; }

        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("title_tag")]
        public TitleTag TitleTag { get; set; }
    }
}
