namespace Shinden.Models
{
    public interface IReadSpeed
    {
         ulong? MangaReadTime { get; }
         double? MangaProc { get; }
         double? VnProc { get; }
    }
}