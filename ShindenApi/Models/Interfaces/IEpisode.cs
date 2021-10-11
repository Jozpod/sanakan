using System;

namespace Shinden.Models
{
    public interface IEpisode : IIndexable
    {
        ulong AnimeId { get; }
        bool IsFiller { get; }
        bool HasOnline { get; }
        string AnimeUrl { get; }
        EpisodeType Type { get; }
        DateTime AirDate { get; }
        string AirChannel { get; }
        string EpisodeUrl { get; }
        ulong EpisodeNumber { get; }
        TimeSpan EpisodeLength { get; }
        IAlternativeTitle EpisodeTitle { get; }
    }
}
