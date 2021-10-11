namespace Shinden.Models.Initializers
{
    public class InitTag
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public ulong? ParentId { get; set; }
        public bool? IsAccepted { get; set; }
        public string NationalName { get; set; }
    }
}