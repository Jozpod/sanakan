using Discord.Commands;
using Sanakan.Common.Models;
using System;
using System.Threading.Tasks;

namespace Sanakan.TypeReaders
{
    /// <inheritdoc/>
    public class ProfileTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(
            ICommandContext context,
            string input,
            IServiceProvider services)
        {
            switch (input.ToLower())
            {
                case "0":
                case "stats":
                case "statystyki":
                    return Task.FromResult(TypeReaderResult.FromSuccess(ProfileType.Statistics));

                case "1":
                case "image":
                case "obrazek":
                    return Task.FromResult(TypeReaderResult.FromSuccess(ProfileType.Image));

                case "2":
                case "ugly":
                case "brzydkie":
                    return Task.FromResult(TypeReaderResult.FromSuccess(ProfileType.StatisticsWithImage));

                case "3":
                case "cards":
                case "karcianka":
                    return Task.FromResult(TypeReaderResult.FromSuccess(ProfileType.Cards));

                default:
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Nie rozpoznano typu nastawy!"));
            }
        }
    }
}