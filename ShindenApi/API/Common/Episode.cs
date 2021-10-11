using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shinden.API
{
    public class Episode
    {
        [JsonProperty("episode_id")]
        public string EpisodeId { get; set; }
        [JsonProperty("episode_title")]
        public string EpisodeTitle { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("is_filer")]
        public string IsFiler { get; set; }
        [JsonProperty("is_special")]
        public string IsSpecial { get; set; }
        [JsonProperty("title_id")]
        public string TitleId { get; set; }
        [JsonProperty("title_main_id")]
        public string TitleMainId { get; set; }
        [JsonProperty("episode_time")]
        public string EpisodeTime { get; set; }
        [JsonProperty("episode_no")]
        public string EpisodeNo { get; set; }
        [JsonProperty("air_date")]
        public string AirDate { get; set; }
        [JsonProperty("air_channell")]
        public string AirChannell { get; set; }
        [JsonProperty("is_accepted")]
        public string IsAccepted { get; set; }
        [JsonProperty("episode_title_id")]
        public string EpisodeTitleId { get; set; }
        [JsonProperty("title_type")]
        public string TitleType { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("lang")]
        public string Lang { get; set; }
        [JsonProperty("langs")]
        public List<string> Langs { get; set; }
        [JsonProperty("has_online")]
        public string HasOnline { get; set; }
    }
}