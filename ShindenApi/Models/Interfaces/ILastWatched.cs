using System;

namespace Shinden.Models
{
    public interface ILastWatched
    {
        uint ViewCnt { get; }
        bool IsFiler { get; }
        ulong TitleId { get; }
        bool IsSpecial { get; }
        long EpisodeNo { get; }
        AnimeType Type { get; }
        string AnimeUrl { get; }
        ulong EpisodeId { get; }
        long EpisodesCnt { get; }
        string EpisodeUrl { get; }
        string AnimeTitle { get; }
        string EpisodeTitle { get; }
        DateTime WatchedDate { get; }
        string AnimeCoverUrl { get; }
    }
}