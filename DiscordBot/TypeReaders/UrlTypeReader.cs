using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Sanakan.TypeReaders
{
    /// <inheritdoc/>
    public class UrlTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(
            ICommandContext context,
            string input,
            IServiceProvider services)
        {
            if (Uri.TryCreate(input, UriKind.Absolute, out var url))
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(url));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Bledny format linku!"));
        }
    }
}