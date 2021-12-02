using Discord.Commands;
using Sanakan.Game.Models;
using System;
using System.Threading.Tasks;

namespace Sanakan.TypeReaders
{
    /// <inheritdoc/>
    public class HaremTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            switch (input.ToLower())
            {
                case "rarity":
                case "jakość":
                case "jakośc":
                case "jakosc":
                case "jakosć":
                    return Task.FromResult(TypeReaderResult.FromSuccess(HaremType.Rarity));

                case "def":
                case "obrona":
                case "defence":
                    return Task.FromResult(TypeReaderResult.FromSuccess(HaremType.Defence));

                case "atk":
                case "atak":
                case "attack":
                    return Task.FromResult(TypeReaderResult.FromSuccess(HaremType.Attack));

                case "cage":
                case "klatka":
                    return Task.FromResult(TypeReaderResult.FromSuccess(HaremType.Cage));

                case "relacja":
                case "affection":
                    return Task.FromResult(TypeReaderResult.FromSuccess(HaremType.Affection));

                case "hp":
                case "życie":
                case "zycie":
                case "health":
                    return Task.FromResult(TypeReaderResult.FromSuccess(HaremType.Health));

                case "tag":
                case "tag+":
                    return Task.FromResult(TypeReaderResult.FromSuccess(HaremType.Tag));

                case "tag-":
                    return Task.FromResult(TypeReaderResult.FromSuccess(HaremType.NoTag));

                case "blocked":
                case "inconvertible":
                case "niewymienialne":
                    return Task.FromResult(TypeReaderResult.FromSuccess(HaremType.Blocked));

                case "broken":
                case "uszkodzone":
                    return Task.FromResult(TypeReaderResult.FromSuccess(HaremType.Broken));

                case "image":
                case "obrazek":
                case "picture":
                    return Task.FromResult(TypeReaderResult.FromSuccess(HaremType.Picture));

                case "image-":
                case "obrazek-":
                case "picture-":
                    return Task.FromResult(TypeReaderResult.FromSuccess(HaremType.NoPicture));

                case "imagec":
                case "obrazekc":
                case "picturec":
                    return Task.FromResult(TypeReaderResult.FromSuccess(HaremType.CustomPicture));

                case "unikat":
                case "unique":
                    return Task.FromResult(TypeReaderResult.FromSuccess(HaremType.Unique));

                default:
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Nie rozpoznano typu haremu!"));
            }
        }
    }
}
