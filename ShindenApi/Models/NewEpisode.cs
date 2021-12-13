using Sanakan.ShindenApi.Converters;
using Sanakan.ShindenApi.Models.Enums;
using Sanakan.ShindenApi.Utilities;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sanakan.ShindenApi.Models
{
    public class NewEpisodeRoot
    {
        [JsonPropertyName("lastonline")]
        public List<NewEpisode> LastOnline { get; set; } = new();
    }

    public class NewEpisode
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = null;

        [JsonPropertyName("title_id")]
        public ulong TitleId { get; set; }

        [JsonPropertyName("cover_artifact_id")]
        public ulong CoverId { get; set; }

        [JsonPropertyName("episode_no")]
        public long EpisodeNumber { get; set; }

        [JsonPropertyName("episode_time")]
        [JsonConverter(typeof(TimeSpanFromMinutesConverter))]
        public TimeSpan? EpisodeLength { get; set; }

        [JsonPropertyName("episode_id")]
        public ulong EpisodeId { get; set; }

        [JsonPropertyName("sub_lang")]
        public Language SubtitlesLanguage { get; set; }

        [JsonPropertyName("langs")]
        public string[] Langs { get; set; } = null;

        [JsonPropertyName("add_date")]
        [JsonConverter(typeof(DateTimeyyyyMMddHHmmssConverter))]
        public DateTime? AddDate { get; set; }

        public string AnimeUrl => UrlHelpers.GetSeriesURL(TitleId);
        public string EpisodeUrl => UrlHelpers.GetEpisodeURL(TitleId, EpisodeId);
        public string AnimeCoverUrl => UrlHelpers.GetBigImageURL(CoverId);
    }
}