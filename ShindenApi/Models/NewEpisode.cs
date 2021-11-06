using Sanakan.ShindenApi.Utilities;
using Shinden.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shinden.API
{
    public class NewEpisodeRoot
    {
        [JsonPropertyName("lastonline")]
        public List<NewEpisode> LastOnline { get; set; }
    }

    public class NewEpisode
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("title_id")]
        public ulong TitleId { get; set; }

        [JsonPropertyName("cover_artifact_id")]
        public ulong CoverId { get; set; }

        [JsonPropertyName("episode_no")]
        public long EpisodeNumber { get; set; }

        [JsonPropertyName("episode_time")]
        public TimeSpan EpisodeLength { get; set; }
        //ulong.TryParse(episode?.EpisodeTime, out var min);
        //return TimeSpan.FromMinutes(min);

        [JsonPropertyName("episode_id")]
        public ulong EpisodeId { get; set; }

        [JsonPropertyName("sub_lang")]
        public Language SubtitlesLanguage { get; set; }
        //return new Language().Parse((episode?.SubLang ?? "").ToLower());

        [JsonPropertyName("langs")]
        public string[] Langs { get; set; }

        [JsonPropertyName("add_date")]
        public DateTime AddDate { get; set; }
         //return DateTime.ParseExact(episode.AddDate, "yyyy-MM-dd HH:mm:ss",
        //System.Globalization.CultureInfo.InvariantCulture);

        public string AnimeUrl => UrlHelpers.GetSeriesURL(TitleId);
        public string EpisodeUrl => UrlHelpers.GetEpisodeURL(TitleId, EpisodeId);
        public string AnimeCoverUrl => UrlHelpers.GetBigImageURL(CoverId);
    }
}