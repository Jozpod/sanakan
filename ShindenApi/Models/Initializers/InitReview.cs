using System;

namespace Shinden.Models.Initializers
{
    public class InitReview
    {
        public ulong Id { get; set; }
        public long Rating { get; set; }
        public ulong TitleId { get; set; }
        public long RateCount { get; set; }
        public long ReadCount { get; set; }
        public string Content { get; set; }
        public long EnterCount { get; set; }
        public bool IsAbstract { get; set; }
        public ISimpleUser Author { get; set; }
    }
}