using System;

namespace Shinden.Models
{
    public interface IRecommendation : IIndexable
    {
        long Rating { get; }
        long RateCount { get; }
        string Content { get; }
        DateTime AddDate { get; }
        ulong BaseTitleId { get; }
        ISimpleUser Author { get; }
        ISimpleTitleInfo RecommendedTitle { get; }
    }
}