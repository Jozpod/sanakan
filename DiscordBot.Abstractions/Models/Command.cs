using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Abstractions
{
    public class Command
    {
        private readonly int _priority;

        public Command(CommandMatch match, ParseResult result, ICommandContext context, int priority)
        {
            Match = match;
            Result = result;
            Context = context;
            _priority = priority;
        }

        public int GetPriority() => _priority;

        public string GetName() => $"cmd-{Match.Command.Name}";

        public CommandMatch Match { get; }
        public ParseResult Result { get; }
        public ICommandContext Context { get; }

        public async Task<Task<bool>> ExecuteAsync(IServiceProvider provider)
        {
            var result = await Match.ExecuteAsync(Context, Result, provider).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                return Task.FromResult(true);
            }

            throw new Exception(result.ErrorReason);
        }
    }
}
