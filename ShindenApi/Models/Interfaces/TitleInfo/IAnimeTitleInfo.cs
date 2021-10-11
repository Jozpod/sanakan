using System;

namespace Shinden.Models
{
    public interface IAnimeTitleInfo : ITitleInfo
    {
        AnimeType Type { get; }
        string AnimeUrl { get; }
        AnimeStatus Status { get; }
        double? MusicRating { get; }
        TimeSpan EpisodeTime { get; }
        ulong? EpisodesCount { get; }
        double? GraphicRating { get; }
    }
}
