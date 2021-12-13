using Discord;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;

namespace Sanakan.TaskQueue.Messages
{
    public class SafariMessage : BaseMessage
    {
        public SafariMessage() : base(Priority.Low) { }

        public EmbedBuilder Embed { get; set; } = null;

        public IUser Winner { get; set; } = null;

        public Card Card { get; set; } = null;

        public ulong? GuildId { get; set; }

        public ITextChannel TrashChannel { get; set; } = null;

        public IUserMessage Message { get; set; } = null;

        public SafariImage Image { get; set; } = null;
    }
}
