using Discord;
using Discord.Commands;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Modules
{
    public abstract class SanakanModuleBase : ModuleBase, IDisposable
    {
        private IDisposable? _typingState;

        public abstract void Dispose();

        [ExcludeFromCodeCoverage]
        public async Task<IMessageChannel> TryCreateDMChannelAsync(IUser user)
        {
            IMessageChannel channel;

#if DEBUG
            if(user.IsBot || user.IsWebhook)
            {
                channel = Context.Channel;
                return channel;
            }
#endif
            channel = await user.CreateDMChannelAsync();
            return channel;
        }

        protected override async void BeforeExecute(CommandInfo command)
        {
            _typingState = Context.Channel.EnterTypingState();

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
                _typingState?.Dispose();
                _typingState = null;
            }
            catch
            {
            }
        }
    }
}
