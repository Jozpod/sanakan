using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class Description : IDescription
    {
        public Description(InitDescription Init)
        {
            Id = Init.Id;
            TitleId = Init.TitleId;
            Content = Init.Content;
            Language = Init.Language;
        }

        public ulong? TitleId { get; }

        // IIndexable
        public ulong Id { get; }

        // IDescription
        public string Content { get; }
        public Language Language { get; }

        public override string ToString() => Content;
    }
}