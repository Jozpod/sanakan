using System;

namespace Shinden.Models
{
    public interface IReview : IIndexable
    {
        long Rating { get; }
        ulong TitleId { get; }
        long RateCount { get; }
        string Content { get; }
        long ReadCount { get; }
        long EnterCount { get; }
        bool IsAbstract { get; }
        ISimpleUser Author { get; }
    }
}