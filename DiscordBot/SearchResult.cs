using Discord.Commands;
using Sanakan.TaskQueue;
using Sanakan.TaskQueue.Messages;
using System;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot
{
    public class SearchResult
    {
        public IResult Result { get; set; }
        public CommandMatch CommandMatch { get; set; }
        public Priority Priority { get; set; }
        public ParseResult ParseResult { get; set; }
        public ICommandContext Context { get; set; }

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
