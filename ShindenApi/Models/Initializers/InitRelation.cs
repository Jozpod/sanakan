namespace Shinden.Models.Initializers
{
    public class InitRelation
    {
        public string CharacterFirstName { get; set; }
        public string CharacterLastName { get; set; }
        public string StaffFirstName { get; set; }
        public string StaffLastName { get; set; }
        public string CharacterRole { get; set; }
        public string StaffPosition { get; set; }
        public ulong? CharacterId { get; set; }
        public Language Language { get; set; }
        public ulong? StaffId { get; set; }
        public string Title { get; set; }
        public ulong Id { get; set; }
    }
}