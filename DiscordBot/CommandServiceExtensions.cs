using Discord.Commands;
using Sanakan.DiscordBot.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using SearchResult = Sanakan.DiscordBot.Abstractions.SearchResult;

namespace Sanakan.DiscordBot.Extensions
{
    public static class CommandServiceExtensions
    {
        public static async Task<SearchResult> GetExecutableCommandAsync(
            this ICommandService commandService,
            ICommandContext context,
            int argPos,
            IServiceProvider services)
            => await GetExecutableCommandAsync(commandService, context, context.Message.Content.Substring(argPos), services).ConfigureAwait(false);

        public static async Task<SearchResult> GetExecutableCommandAsync(
            this ICommandService commandService,
            ICommandContext context,
            string input,
            IServiceProvider services)
        {
            var searchResult = commandService.Search(input);

            if (!searchResult.IsSuccess)
            {
                return new SearchResult(searchResult);
            }

            var commands = searchResult.Commands;
            var preconditionResults = new Dictionary<CommandMatch, PreconditionResult>();

            foreach (var match in commands)
            {
                preconditionResults[match] = await match.Command
                    .CheckPreconditionsAsync(context, services)
                    .ConfigureAwait(false);
            }

            var successfulPreconditions = preconditionResults
                .Where(x => x.Value.IsSuccess)
                .ToList();

            if (!successfulPreconditions.Any())
            {
                var bestCandidate = preconditionResults
                    .OrderByDescending(x => x.Key.Command.Priority)
                    .FirstOrDefault(x => !x.Value.IsSuccess);

                return new SearchResult(bestCandidate.Value);
            }

            var parseResultsDict = new Dictionary<CommandMatch, ParseResult>();
            foreach (var pair in successfulPreconditions)
            {
                var parseResult = await pair.Key.ParseAsync(context, searchResult, pair.Value, services).ConfigureAwait(false);

                if (parseResult.Error == CommandError.MultipleMatches)
                {
                    IReadOnlyList<TypeReaderValue> argList, paramList;
                    argList = parseResult.ArgValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                    paramList = parseResult.ParamValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                    parseResult = ParseResult.FromSuccess(argList, paramList);
                }

                parseResultsDict[pair.Key] = parseResult;
            }

            float CalculateScore(CommandMatch match, ParseResult parseResult)
            {
                float argValuesScore = 0, paramValuesScore = 0;

                if (match.Command.Parameters.Count > 0)
                {
                    var argValuesSum = parseResult.ArgValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;
                    var paramValuesSum = parseResult.ParamValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;

                    argValuesScore = argValuesSum / match.Command.Parameters.Count;
                    paramValuesScore = paramValuesSum / match.Command.Parameters.Count;
                }

                var totalArgsScore = (argValuesScore + paramValuesScore) / 2;
                return match.Command.Priority + (totalArgsScore * 0.99f);
            }

            var parseResults = parseResultsDict
                .OrderByDescending(x => CalculateScore(x.Key, x.Value));

            var successfulParses = parseResults
                .Where(x => x.Value.IsSuccess)
                .ToArray();

            if (successfulParses.Length == 0)
            {
                var bestMatch = parseResults
                    .FirstOrDefault(x => !x.Value.IsSuccess);

                return new SearchResult(bestMatch.Value);
            }

            var chosenOverload = successfulParses[0];
            return new SearchResult(
                command: new Command(chosenOverload.Key,
                chosenOverload.Value,
                context,
                chosenOverload.Key.Command.Priority));
        }
    }
}
