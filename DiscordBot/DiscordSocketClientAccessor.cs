using Discord.WebSocket;

namespace Sanakan.DiscordBot
{
    internal class DiscordSocketClientAccessor : IDiscordSocketClientAccessor
    {
        public DiscordSocketClient? Client { get; set; }
    }
}
