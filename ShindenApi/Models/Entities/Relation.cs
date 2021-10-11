using Shinden.API;
using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class Relation : IRelation
    {
        public Relation(InitRelation Init)
        {
            Id = Init.Id;
            Title = Init.Title;
            StaffId = Init.StaffId;
            Language = Init.Language;
            CharacterId = Init.CharacterId;
            StaffPosition = Init.StaffPosition;
            CharacterRole = Init.CharacterRole;
            StaffLastName = Init.StaffLastName;
            StaffFirstName = Init.StaffFirstName;
            CharacterLastName = Init.CharacterLastName;
            CharacterFirstName = Init.CharacterFirstName;
        }

        // IIndexable
        public ulong Id { get; }

        // IRelation
        public string CharacterFirstName { get; }
        public string CharacterLastName { get; }
        public string StaffFirstName { get; }
        public string StaffLastName { get; }
        public string CharacterRole { get; }
        public string StaffPosition { get; }
        public ulong? CharacterId { get; }
        public Language Language { get; }
        public ulong? StaffId { get; }
        public string Title { get; }

        public override string ToString() => Title;
    }
}
