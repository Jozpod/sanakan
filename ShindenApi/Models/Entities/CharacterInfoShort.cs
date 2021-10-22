using Sanakan.ShindenApi.Utilities;
using Shinden.API;

namespace Shinden.Models.Entities
{
    public class CharacterInfoShort : ICharacterInfoShort
    {
        public CharacterInfoShort(ulong id, string lastName, string firstName, ulong pictureId)
        {
            Id = id;
            LastName = lastName;
            FirstName = firstName;
            PictureId = pictureId;
        }

        private ulong? PictureId { get; }

        // IIndexable
        public ulong Id { get; }

        // ICharacterInfoShort
        public string LastName { get; }
        public string FirstName { get; }
        public string CharacterUrl => UrlHelpers.GetCharacterURL(Id);
        public string PictureUrl => UrlHelpers.GetPersonPictureURL(PictureId);
        public bool HasImage => PictureUrl != UrlHelpers.GetPlaceholderImageURL();

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
