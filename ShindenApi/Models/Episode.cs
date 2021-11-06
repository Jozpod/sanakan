using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class Episode
    {
        [JsonPropertyName("episode_id")]
        public string EpisodeId { get; set; }

        [JsonPropertyName("episode_title")]
        public string EpisodeTitle { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("is_filer")]
        public string IsFiler { get; set; }

        [JsonPropertyName("is_special")]
        public string IsSpecial { get; set; }

        [JsonPropertyName("title_id")]
        public string TitleId { get; set; }

        [JsonPropertyName("title_main_id")]
        public string TitleMainId { get; set; }


        [JsonPropertyName("episode_time")]
        public string EpisodeTime { get; set; }


        [JsonPropertyName("episode_no")]
        public string EpisodeNo { get; set; }


        [JsonPropertyName("air_date")]
        public string AirDate { get; set; }


        [JsonPropertyName("air_channell")]
        public string AirChannell { get; set; }

        [JsonPropertyName("is_accepted")]
        public string IsAccepted { get; set; }

        [JsonPropertyName("episode_title_id")]
        public string EpisodeTitleId { get; set; }


        [JsonPropertyName("title_type")]
        public string TitleType { get; set; }


        [JsonPropertyName("title")]
        public string Title { get; set; }


        [JsonPropertyName("lang")]
        public string Lang { get; set; }


        [JsonPropertyName("langs")]
        public List<string> Langs { get; set; }


        [JsonPropertyName("has_online")]
        public string HasOnline { get; set; }
    }
}