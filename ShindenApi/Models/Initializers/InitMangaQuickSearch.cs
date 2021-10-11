namespace Shinden.Models.Initializers
{
    public class InitMangaQuickSearch
    {
        public string Title { get; set; }
        public ulong MangaId { get; set; }
        public ulong? CoverId { get; set; }
        public MangaType Type { get; set; }
        public MangaStatus Status { get; set; }
    }
}
