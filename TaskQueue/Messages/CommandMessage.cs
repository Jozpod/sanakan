﻿using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Messages
{
    public class CommandMessage : BaseMessage
    {
        public CommandMessage(CommandMatch commandMatch, Priority priority)
            : base(priority)
        {
            Match = new CommandMatchWrapper(commandMatch);
        }

        public interface ICommandMatchWrapper
        {
            Task<IResult> ExecuteAsync(ICommandContext context, ParseResult parseResult, IServiceProvider services);
        }

        public ICommandMatchWrapper Match { get; set; } = null;

        public ParseResult ParseResult { get; set; }

        public ICommandContext Context { get; set; } = null;

        public class CommandMatchWrapper : ICommandMatchWrapper
        {
            private readonly CommandMatch _commandMatch;

            public CommandMatchWrapper(CommandMatch commandMatch)
            {
                _commandMatch = commandMatch;
            }

            public Task<IResult> ExecuteAsync(ICommandContext context, ParseResult parseResult, IServiceProvider services)
                => _commandMatch.ExecuteAsync(context, parseResult, services);
        }
    }
}
