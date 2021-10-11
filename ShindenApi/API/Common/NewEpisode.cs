using Newtonsoft.Json;

namespace Shinden.API
{
    public class NewEpisode
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("title_id")]
        public string TitleId { get; set; }
        [JsonProperty("cover_artifact_id")]
        public string Cover_artifact_id { get; set; }
        [JsonProperty("episode_no")]
        public string EpisodeNo { get; set; }
        [JsonProperty("episode_time")]
        public string EpisodeTime { get; set; }
        [JsonProperty("episode_id")]
        public string EpisodeId { get; set; }
        [JsonProperty("sub_lang")]
        public string SubLang { get; set; }
        [JsonProperty("langs")]
        public string[] Langs { get; set; }
        [JsonProperty("add_date")]
        public string AddDate { get; set; }
    }
}