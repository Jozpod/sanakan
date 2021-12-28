using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Modules
{
    public abstract class SanakanModuleBase : ModuleBase, IDisposable
    {
        private IDisposable? _typingState;

        public abstract void Dispose();

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
