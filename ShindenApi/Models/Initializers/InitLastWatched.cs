using System;

namespace Shinden.Models.Entities
{
    public class InitLastWatched
    {
        public uint ViewCnt { get; set; }
        public bool IsFiler { get; set; }
        public ulong TitleId { get; set; }
        public bool IsSpecial { get; set; }
        public long EpisodeNo { get; set; }
        public AnimeType Type { get; set; }
        public ulong EpisodeId { get; set; }
        public long EpisodesCnt { get; set; }
        public string AnimeTitle { get; set; }
        public ulong? TitleCoverId { get; set; }
        public string EpisodeTitle { get; set; }
        public DateTime WatchedDate { get; set; }
    }
}
