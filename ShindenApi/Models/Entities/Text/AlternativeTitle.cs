using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class AlternativeTitle : IAlternativeTitle
    {
        public AlternativeTitle(InitAlternativeTitle Init)
        {
            Id = Init.Id;
            Type = Init.Type;
            TitleId = Init.TitleId;
            Content = Init.Content;
            Language = Init.Language;
            IsAccepted = Init.IsAccepted;
        }

        public ulong? TitleId { get; }
        public bool? IsAccepted { get; }

        // IIndexable
        public ulong Id { get; }

        // IAlternativeTitle
        public string Content { get; }
        public Language Language { get; }
        public AlternativeTitleType Type { get; }

        public override string ToString() => Content;
    }
}