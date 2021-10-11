using System;

namespace Shinden.Models.Initializers
{
    public class InitEpisode
    {
        public ulong Id { get; set; }
        public ulong AnimeId { get; set; }
        public bool IsFiller { get; set; }
        public bool HasOnline { get; set; }
        public ulong OnlineId { get; set; }
        public EpisodeType Type { get; set; }
        public DateTime AirDate { get; set; }
        public string AirChannel { get; set; }
        public ulong EpisodeNumber { get; set; }
        public TimeSpan EpisodeLength { get; set; }
        public IAlternativeTitle EpisodeTitle { get; set; }
    }
}
