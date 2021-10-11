namespace Shinden.Models.Entities
{
    public class ReadSpeed : IReadSpeed
    {
        public ReadSpeed(ulong? MangaReadTime, double? MangaProc, double? VnProc)
        {
            this.MangaReadTime = MangaReadTime;
            this.MangaProc = MangaProc;
            this.VnProc = VnProc;
        }

        // ReadSpeed
        public ulong? MangaReadTime { get; }
        public double? MangaProc { get; }
        public double? VnProc { get; }
    }
}
