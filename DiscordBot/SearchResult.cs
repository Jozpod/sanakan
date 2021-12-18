using Discord.Commands;
using Sanakan.TaskQueue;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot
{
    [ExcludeFromCodeCoverage]
    public class SearchResult
    {
        public IResult Result { get; set; } = null;

        public CommandMatch CommandMatch { get; set; }

        public Priority Priority { get; set; }

        public ParseResult ParseResult { get; set; }

        public ICommandContext Context { get; set; } = null;

        public Task<IResult> ExecuteAsync(IServiceProvider serviceProvider) => CommandMatch.ExecuteAsync(Context, ParseResult, serviceProvider);

        public bool IsSuccess()
        {
            if (Result == null)
            {
                return false;
            }

            return Result.IsSuccess;
        }
    }
}
