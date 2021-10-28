using Discord.WebSocket;

namespace Sanakan.DiscordBot
{
    public interface IDiscordSocketClientAccessor
    {
        public DiscordSocketClient? Client { get; set; }
    }
}
