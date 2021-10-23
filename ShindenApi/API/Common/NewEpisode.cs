using Newtonsoft.Json;
using Sanakan.ShindenApi.Utilities;
using Shinden.Models;
using System;

namespace Shinden.API
{
    public class NewEpisode
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_id")]
        public ulong TitleId { get; set; }

        [JsonProperty("cover_artifact_id")]
        public ulong CoverId { get; set; }

        [JsonProperty("episode_no")]
        public long EpisodeNumber { get; set; }

        [JsonProperty("episode_time")]
        public TimeSpan EpisodeLength { get; set; }
        //ulong.TryParse(episode?.EpisodeTime, out var min);
        //return TimeSpan.FromMinutes(min);

        [JsonProperty("episode_id")]
        public ulong EpisodeId { get; set; }

        [JsonProperty("sub_lang")]
        public Language SubtitlesLanguage { get; set; }

        [JsonProperty("langs")]
        public string[] Langs { get; set; }

        [JsonProperty("add_date")]
        public DateTime AddDate { get; set; }

        public string AnimeUrl => UrlHelpers.GetSeriesURL(TitleId);
        public string EpisodeUrl => UrlHelpers.GetEpisodeURL(TitleId, EpisodeId);
        public string AnimeCoverUrl => UrlHelpers.GetBigImageURL(CoverId);
    }
}