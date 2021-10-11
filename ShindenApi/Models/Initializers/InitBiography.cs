namespace Shinden.Models.Initializers
{
    public class InitBiography
    {
        public ulong Id { get; set; }
        public string Content { get; set; }
        public ulong RelatedId { get; set; }
        public Language Language { get; set; }
    }
}