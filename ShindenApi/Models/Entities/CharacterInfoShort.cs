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
        public string CharacterUrl => Url.GetCharacterURL(Id);
        public string PictureUrl => Url.GetPersonPictureURL(PictureId);
        public bool HasImage => PictureUrl != Url.GetPlaceholderImageURL();

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
