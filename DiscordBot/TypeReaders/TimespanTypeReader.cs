using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Sanakan.TypeReaders
{
    /// <inheritdoc/>
    public class TimespanTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (TimeSpan.TryParse(input, out var timespan))
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(timespan));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Bledny format czasu."));
        }
    }
}