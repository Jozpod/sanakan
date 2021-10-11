using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class CharacterBiography : ICharacterBiography
    {
        public CharacterBiography(InitBiography Init)
        {
            Id = Init.Id;
            Content = Init.Content;
            Language = Init.Language;
            CharacterId = Init.RelatedId;
        }

        // IIndexable
        public ulong Id { get; }

        // ICharacterBiography
        public string Content { get; }
        public ulong CharacterId { get; }
        public Language Language { get; }

        public override string ToString() => Content;
    }
}