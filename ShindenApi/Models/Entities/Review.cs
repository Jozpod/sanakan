using System;
using Shinden.API;
using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class Review : IReview
    {
        public Review(InitReview Init)
        {
            Id = Init.Id;
            Author = Init.Author;
            Rating = Init.Rating;
            TitleId = Init.TitleId;
            Content = Init.Content;
            RateCount = Init.RateCount;
            ReadCount = Init.ReadCount;
            IsAbstract = Init.IsAbstract;
            EnterCount = Init.EnterCount;
        }

        // IIndexable
        public ulong Id { get; }

        // IReview
        public long Rating { get; }
        public ulong TitleId { get; }
        public long RateCount { get; }
        public long ReadCount { get; }
        public string Content { get; }
        public long EnterCount { get; }
        public bool IsAbstract { get; }
        public ISimpleUser Author { get; }

        public override string ToString() => Content;
    }
}
