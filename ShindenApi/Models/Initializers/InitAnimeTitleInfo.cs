using System;
using System.Collections.Generic;

namespace Shinden.Models.Initializers
{
    public class InitAnimeTitleInfo
    {
        public ulong Id { get; set; }
        public bool? DMCA { get; set; }
        public string Title { get; set; }
        public ulong? CoverId { get; set; }
        public AnimeType Type { get; set; }
        public MpaaRating MPAA { get; set; }
        public DateTime AddDate { get; set; }
        public AnimeStatus Status { get; set; }
        public double? StoryRating { get; set; }
        public double? TotalRating { get; set; }
        public double? MusicRating { get; set; }
        public TimeSpan EpisodeTime { get; set; }
        public ulong? EpisodesCount { get; set; }
        public double? GraphicRating { get; set; }
        public double? RankingRating { get; set; }
        public ulong? RankingPosition { get; set; }
        public IDescription Description { get; set; }
        public IDateTimePrecision StartDate { get; set; }
        public IDateTimePrecision FinishDate { get; set; }
        public List<ITagCategory> TagCategories { get; set; }
        public List<IAlternativeTitle> AlternativeTitles { get; set; }
    }
}
