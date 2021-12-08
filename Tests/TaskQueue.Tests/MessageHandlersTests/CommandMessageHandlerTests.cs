using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.TaskQueue.MessageHandlers;
using Sanakan.TaskQueue.Messages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Sanakan.TaskQueue.Messages.CommandMessage;

namespace Sanakan.TaskQueue.Tests.MessageHandlersTests
{
    [TestClass]
    public class CommandMessageHandlerTests
    {
        private readonly CommandMessageHandler _messageHandler;
        private readonly Mock<IServiceProvider> _serviceProviderMock = new(MockBehavior.Strict);
        
        public CommandMessageHandlerTests()
        {
            _messageHandler = new(
                NullLogger<CommandMessageHandler>.Instance,
                _serviceProviderMock.Object);
        }

        [TestMethod]
        public async Task Should_Handle_Message()
        {
            var commandContextMock = new Mock<ICommandContext>(MockBehavior.Strict);
            var commandMatchWrapperMock = new Mock<ICommandMatchWrapper>(MockBehavior.Strict);
            var result = ParseResult.FromSuccess(new List<TypeReaderResult>(), new List<TypeReaderResult>());
            var message = new CommandMessage(new CommandMatch(), Priority.Medium)
            {
                Match = commandMatchWrapperMock.Object,
                ParseResult = result,
                Context = commandContextMock.Object,
            };

            commandMatchWrapperMock
                .Setup(pr => pr.ExecuteAsync(message.Context, message.ParseResult, _serviceProviderMock.Object))
                .ReturnsAsync(result);
            
            await _messageHandler.HandleAsync(message);
        }
    }
}
