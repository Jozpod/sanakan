using System;

namespace Shinden.Models.Initializers
{
    public class InitNewEpisode
    {
        public ulong AnimeId { get; set; }
        public ulong? CoverId { get; set; }
        public ulong EpisodeId { get; set; }
        public DateTime AddDate { get; set; }
        public string AnimeTitle { get; set; }
        public ulong EpisodeNumber { get; set; }
        public TimeSpan EpisodeLength { get; set; }
        public Language SubtitlesLanguage { get; set; }
    }
}
