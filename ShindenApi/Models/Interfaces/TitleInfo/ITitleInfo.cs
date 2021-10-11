using System;
using System.Collections.Generic;

namespace Shinden.Models
{
    public interface ITitleInfo : ISimpleTitleInfo
    {
        MpaaRating MPAA { get; }
        DateTime AddDate { get; }
        double? StoryRating { get; }
        double? TotalRating { get; }
        double? RankingRating { get; }
        ulong? RankingPosition { get; }
        IDescription Description { get; }
        IDateTimePrecision StartDate { get; }
        IDateTimePrecision FinishDate { get; }
        List<ITagCategory> TagCategories { get; }
        List<IAlternativeTitle> AlternativeTitles { get; }
    }
}
