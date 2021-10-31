using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot
{
    internal class DiscordSocketClientAccessor : IDiscordSocketClientAccessor
    {
        private DiscordSocketClient? _client;
        public DiscordSocketClient? Client
        {
            get
            {
                return _client;
            }
            set
            {
                _client = value;
                Initialized.Invoke();
            }
        }

        public event Func<Task> Initialized;
    }
}
