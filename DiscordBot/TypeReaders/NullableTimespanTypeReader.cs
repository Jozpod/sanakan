using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Sanakan.TypeReaders
{
    /// <inheritdoc/>
    public class NullableTimespanTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            TimeSpan.TryParse(input, out var timespan);
            return Task.FromResult(TypeReaderResult.FromSuccess(timespan));
        }
    }
}