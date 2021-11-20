using Discord.Commands;
using Sanakan.DiscordBot.Services;
using System;
using System.Threading.Tasks;

namespace Sanakan.TypeReaders
{
    public class CoinSideTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(
            ICommandContext context,
            string input,
            IServiceProvider services)
        {
            switch (input.ToLower())
            {
                case "orzel":
                case "head":
                case "orzeł":
                    return Task.FromResult(TypeReaderResult.FromSuccess(CoinSide.Head));

                case "tail":
                case "reszka":
                    return Task.FromResult(TypeReaderResult.FromSuccess(CoinSide.Tail));

                default:
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Nie rozpoznano strony monety!"));
            }
        }
    }
}