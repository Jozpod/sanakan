using Discord;
using Discord.WebSocket;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;

namespace Sanakan.TaskQueue.Messages
{
    public class SafariMessage : BaseMessage
    {
        public SafariMessage() : base(Priority.Low) {}

        public EmbedBuilder Embed { get; set; }
        public IUser Winner { get; set; }
        public Card Card { get; set; }
        public ulong? GuildId { get; set; }
        public ITextChannel TrashChannel { get; set; }
        public IUserMessage Message { get; set; }
        public SafariImage Image { get; set; }
    }
}
