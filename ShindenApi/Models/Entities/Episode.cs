using Shinden.API;
using Shinden.Models.Initializers;
using System;

namespace Shinden.Models.Entities
{
    public class Episode : IEpisode
    {
        public Episode(InitEpisode Init)
        {
            Id = Init.Id;
            Type = Init.Type;
            AnimeId = Init.AnimeId;
            AirDate = Init.AirDate;
            IsFiller = Init.IsFiller;
            HasOnline = Init.HasOnline;
            AirChannel = Init.AirChannel;
            EpisodeTitle = Init.EpisodeTitle;
            EpisodeNumber = Init.EpisodeNumber;
            EpisodeLength = Init.EpisodeLength;
        }
        
        // IIndexable
        public ulong Id { get; }

        // IEpisode
        public ulong AnimeId { get; }
        public bool IsFiller { get; }
        public bool HasOnline { get; }
        public EpisodeType Type { get; }
        public DateTime AirDate { get; }
        public string AirChannel { get; }
        public ulong EpisodeNumber { get; }
        public TimeSpan EpisodeLength { get; }
        public IAlternativeTitle EpisodeTitle { get; }

        public string AnimeUrl => Url.GetSeriesURL(AnimeId);
        public string EpisodeUrl => Url.GetEpisodeURL(AnimeId, Id);

        public override string ToString() => EpisodeTitle?.ToString() ?? $"{EpisodeNumber}";
    }
}
