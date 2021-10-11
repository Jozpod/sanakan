namespace Shinden.Models
{
    public interface IRelation : IIndexable
    {
        string CharacterFirstName { get; }
        string CharacterLastName { get; }
        string StaffFirstName { get; }
        string StaffLastName { get; }
        string CharacterRole { get; }
        string StaffPosition { get; }
        ulong? CharacterId { get; }
        Language Language { get; }
        ulong? StaffId { get; }
        string Title { get; }
    }
}