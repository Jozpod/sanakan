namespace Shinden.Models.Initializers
{
    public class InitPicture
    {
        public PictureType Type { get; set; }
        public ulong PictureId { get; set; }
        public bool IsAccepted { get; set; }
        public bool Is18Plus { get; set; }
        public string Title { get; set; }
    }
}