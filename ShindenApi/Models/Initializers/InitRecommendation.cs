using System;

namespace Shinden.Models.Initializers
{
    public class InitRecommendation
    {
        public ulong Id { get; set; }
        public long Rating { get; set; }
        public long RateCount { get; set; }
        public string Content { get; set; }
        public DateTime AddDate { get; set; }
        public ulong BaseTitleId { get; set; }
        public ISimpleUser Author { get; set; }
        public ISimpleTitleInfo RecommendedTitle { get; set; }
    }
}