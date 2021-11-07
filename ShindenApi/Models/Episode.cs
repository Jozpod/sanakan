using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class Episode
    {
        [JsonPropertyName("episode_id")]
        public ulong EpisodeId { get; set; }

        [JsonPropertyName("episode_title")]
        public string EpisodeTitle { get; set; }

        [JsonPropertyName("type")]
        public EpisodeType Type { get; set; }

        [JsonPropertyName("is_filer")]
        public bool IsFiler { get; set; }

        [JsonPropertyName("is_special")]
        public bool IsSpecial { get; set; }

        [JsonPropertyName("title_id")]
        public ulong TitleId { get; set; }

        [JsonPropertyName("title_main_id")]
        public string TitleMainId { get; set; }

        [JsonPropertyName("episode_time")]
        [JsonConverter(typeof(TimeSpanFromMinutesConverter))]
        public TimeSpan? EpisodeTime { get; set; }

        [JsonPropertyName("episode_no")]
        public ulong EpisodeNo { get; set; }

        [JsonPropertyName("air_date")]
        [JsonConverter(typeof(DateTimeyyyyMMddConverter))]
        public DateTime? AirDate { get; set; }

        [JsonPropertyName("air_channell")]
        public string AirChannell { get; set; }

        [JsonPropertyName("is_accepted")]
        public bool IsAccepted { get; set; }

        [JsonPropertyName("episode_title_id")]
        public ulong EpisodeTitleId { get; set; }

        [JsonPropertyName("title_type")]
        public string TitleType { get; set; }

        [JsonPropertyName("title")]
        [JsonConverter(typeof(HtmlDecodeAndRemoveBBCodeConverter))]
        public string Title { get; set; }

        [JsonPropertyName("lang")]
        public Language Lang { get; set; }

        [JsonPropertyName("langs")]
        public List<string> Langs { get; set; }

        [JsonPropertyName("has_online")]
        public bool HasOnline { get; set; }
    }
}