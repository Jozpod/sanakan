using Discord.Commands;

namespace Sanakan.DiscordBot.Abstractions
{
    public class SearchResult
    {
        public SearchResult(IResult? result = null, Command? command = null)
        {
            Result = result;
            Command = command;
        }

        public IResult Result { get; }
        public Command Command { get; }

        public bool IsSuccess()
        {
            if (Command == null)
            {
                if (Result == null)
                    return false;

                return Result.IsSuccess;
            }

            return true;
        }
    }
}
