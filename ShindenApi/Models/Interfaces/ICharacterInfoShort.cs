namespace Shinden.Models
{
    public interface ICharacterInfoShort : IIndexable
    {
        bool HasImage { get; }
        string LastName { get; }
        string FirstName { get; }
        string PictureUrl { get; }
        string CharacterUrl { get; }
    }
}