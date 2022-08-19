using Discord.Commands;
using Sanakan.Game.Models;
using System;
using System.Threading.Tasks;

namespace Sanakan.TypeReaders
{
    /// <inheritdoc/>
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