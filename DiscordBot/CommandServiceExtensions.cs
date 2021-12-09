using Discord.Commands;
using Sanakan.TaskQueue;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

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
            ICommandContext commandContext,
            string input,
            IServiceProvider services)
        {
            var searchResult = commandService.Search(input);

            if (!searchResult.IsSuccess)
            {
                return new SearchResult
                {
                    Result = searchResult,
                };
            }

            var commands = searchResult.Commands;
            var preconditionResults = new Dictionary<CommandMatch, PreconditionResult>();

            foreach (var match in commands)
            {
                preconditionResults[match] = await match.Command
                    .CheckPreconditionsAsync(commandContext, services)
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

                return new SearchResult
                {
                    Result = bestCandidate.Value,
                };
            }

            var parseResultsDict = new Dictionary<CommandMatch, ParseResult>();
            foreach (var pair in successfulPreconditions)
            {
                var parseResult = await pair.Key.ParseAsync(commandContext, searchResult, pair.Value, services).ConfigureAwait(false);

                if (parseResult.Error == CommandError.MultipleMatches)
                {
                    IReadOnlyList<TypeReaderValue> argList, paramList;
                    argList = parseResult.ArgValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                    paramList = parseResult.ParamValues.Select(x => x.Values.OrderByDescending(y => y.Score).First()).ToImmutableArray();
                    parseResult = ParseResult.FromSuccess(argList, paramList);
                }

                parseResultsDict[pair.Key] = parseResult;
            }

            var parseResults = parseResultsDict
                .OrderByDescending(x => CalculateScore(x.Key, x.Value));

            var successfulParses = parseResults
                .Where(x => x.Value.IsSuccess)
                .ToList();

            if (!successfulParses.Any())
            {
                var bestMatch = parseResults
                    .FirstOrDefault(x => !x.Value.IsSuccess);

                return new SearchResult
                {
                    Result = bestMatch.Value,
                };
            }

            var chosenOverload = successfulParses.First();
            var commandMatch = chosenOverload.Key;
            var priority = (Priority)commandMatch.Command.Priority;

            return new SearchResult()
            {
                Priority = priority,
                CommandMatch = commandMatch,
                Context = commandContext,
                Result = chosenOverload.Value,
                ParseResult = chosenOverload.Value,
            };
        }

        private static float CalculateScore(CommandMatch match, ParseResult parseResult)
        {
            float argValuesScore = 0, paramValuesScore = 0;
            var command = match.Command;
            var parametersCount = command.Parameters.Count;

            if (parametersCount > 0)
            {
                var argValuesSum = parseResult.ArgValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;
                var paramValuesSum = parseResult.ParamValues?.Sum(x => x.Values.OrderByDescending(y => y.Score).FirstOrDefault().Score) ?? 0;

                argValuesScore = argValuesSum / parametersCount;
                paramValuesScore = paramValuesSum / parametersCount;
            }

            var totalArgsScore = (argValuesScore + paramValuesScore) / 2;
            return command.Priority + (totalArgsScore * 0.99f);
        }
    }
}
