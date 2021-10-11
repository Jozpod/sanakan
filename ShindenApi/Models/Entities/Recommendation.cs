using System;
using Shinden.API;
using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class Recommendation : IRecommendation
    {
        public Recommendation(InitRecommendation Init)
        {
            Id = Init.Id;
            Author = Init.Author;
            Rating = Init.Rating;
            Content = Init.Content;
            AddDate = Init.AddDate;
            RateCount = Init.RateCount;
            BaseTitleId = Init.BaseTitleId;
            RecommendedTitle = Init.RecommendedTitle;
        }

        // IIndexable
        public ulong Id { get; }

        // IRecommendation
        public long Rating { get; }
        public long RateCount { get; }
        public string Content { get; }
        public DateTime AddDate { get; }
        public ulong BaseTitleId { get; }
        public ISimpleUser Author { get; }
        public ISimpleTitleInfo RecommendedTitle { get; }

        public override string ToString() => RecommendedTitle.ToString();
    }
}
