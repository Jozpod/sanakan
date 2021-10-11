namespace Shinden.Models.Initializers
{
    public class InitDescription
    {
        public ulong Id { get; set; }
        public ulong? TitleId { get; set; }
        public string Content { get; set; }
        public Language Language { get; set; }
    }
}