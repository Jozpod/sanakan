using Shinden.API;
using Shinden.Models.Initializers;

namespace Shinden.Models.Entities
{
    public class Picture : IPicture
    {
        public Picture(InitPicture Init)
        {
            IsAccepted = Init.IsAccepted;
            PictureId = Init.PictureId;
            Is18Plus = Init.Is18Plus;
            Title = Init.Title;
            Type = Init.Type;
        }

        // IPicture
        public PictureType Type { get; }
        public ulong PictureId { get; }
        public bool IsAccepted { get; }
        public bool Is18Plus { get; }
        public string Title { get; }

        public override string ToString() => Title;
    }
}
