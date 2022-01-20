using Discord.Commands;
using Sanakan.DAL.Models;
using System;
using System.Threading.Tasks;

namespace Sanakan.TypeReaders
{
    /// <inheritdoc/>
    public class ExpeditionTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            TypeReaderResult result;

            switch (input.ToLower())
            {
                case "-":
                    result = TypeReaderResult.FromSuccess(ExpeditionCardType.None);
                    break;
                case "normalna":
                case "normal":
                case "n":
                    result = TypeReaderResult.FromSuccess(ExpeditionCardType.NormalItemWithExp);
                    break;
                case "trudna":
                case "hard":
                case "h":
                    result = TypeReaderResult.FromSuccess(ExpeditionCardType.ExtremeItemWithExp);
                    break;
                case "mrok":
                case "dark":
                case "d":
                    result = TypeReaderResult.FromSuccess(ExpeditionCardType.DarkItemWithExp);
                    break;
                case "mrok 1":
                case "dark 1":
                case "d1":
                    result = TypeReaderResult.FromSuccess(ExpeditionCardType.DarkExp);
                    break;
                case "mrok 2":
                case "dark 2":
                case "d2":
                    result = TypeReaderResult.FromSuccess(ExpeditionCardType.DarkItems);
                    break;
                case "światło":
                case "światlo":
                case "swiatło":
                case "swiatlo":
                case "light":
                case "l":
                    result = TypeReaderResult.FromSuccess(ExpeditionCardType.LightItemWithExp);
                    break;
                case "światło 1":
                case "światlo 1":
                case "swiatło 1":
                case "swiatlo 1":
                case "light 1":
                case "l1":
                    result = TypeReaderResult.FromSuccess(ExpeditionCardType.LightExp);
                    break;
                case "światło 2":
                case "światlo 2":
                case "swiatło 2":
                case "swiatlo 2":
                case "light 2":
                case "l2":
                    result = TypeReaderResult.FromSuccess(ExpeditionCardType.LightItems);
                    break;
                case "ue":
                case "sandbox":
                case "piaskownica":
                    result = TypeReaderResult.FromSuccess(ExpeditionCardType.UltimateEasy);
                    break;
                case "um":
                case "spacer":
                case "stroll":
                    result = TypeReaderResult.FromSuccess(ExpeditionCardType.UltimateMedium);
                    break;
                case "uh":
                case "sprint":
                    result = TypeReaderResult.FromSuccess(ExpeditionCardType.UltimateHard);
                    break;
                case "uhh":
                case "maraton":
                case "marathon":
                    result = TypeReaderResult.FromSuccess(ExpeditionCardType.UltimateHardcore);
                    break;
                default:
                    result = TypeReaderResult.FromError(CommandError.ParseFailed, "Nie rozpoznano typu wyprawy!");
                    break;
            }

            return Task.FromResult(result);
        }
    }
}
