﻿using Discord.Commands;
using Sanakan.Game.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Sanakan.TypeReaders
{
    /// <inheritdoc/>
    public class TopTypeReader : TypeReader
    {
        [SuppressMessage("Microsoft.Analyzers.ManagedCodeAnalysis", "CA1502:AvoidExcessiveComplexity", Justification = "Resolved")]
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            switch (input.ToLower())
            {
                case "lvl":
                case "level":
                case "poziom":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.Level));

                case "sc":
                case "funds":
                case "wallet":
                case "wallet sc":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.ScCount));

                case "tc":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.TcCount));

                case "ac":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.AcCount));

                case "pc":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.PcCount));

                case "posty":
                case "msg":
                case "wiadomosci":
                case "wiadomości":
                case "messages":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.Posts));

                case "postym":
                case "msgm":
                case "wiadomoscim":
                case "wiadomościm":
                case "messagesm":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.PostsMonthly));

                case "postyms":
                case "msgmavg":
                case "wiadomoscims":
                case "wiadomościms":
                case "messagesmabg":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.PostsMonthlyCharacter));

                case "command":
                case "commands":
                case "polecenia":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.Commands));

                case "karta":
                case "card":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.Card));

                case "karty":
                case "cards":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.Cards));

                case "kartym":
                case "cardsp":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.CardsPower));

                case "karma":
                case "karma+":
                case "+karma":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.Karma));

                case "karma-":
                case "-karma":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.KarmaNegative));

                case "pvp":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.Pvp));

                case "pvps":
                    return Task.FromResult(TypeReaderResult.FromSuccess(TopType.PvpSeason));

                default:
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Nie rozpoznano typu topki!"));
            }
        }
    }
}
