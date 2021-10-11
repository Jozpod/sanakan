namespace Shinden.Models
{
    public interface IPicture
    {
        PictureType Type { get; }
        ulong PictureId { get; }
        bool IsAccepted { get; }
        bool Is18Plus { get; }
        string Title { get; }
    }
}