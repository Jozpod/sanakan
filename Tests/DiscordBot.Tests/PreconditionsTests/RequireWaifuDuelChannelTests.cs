using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.Preconditions;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Test
{
    [TestClass]
    public class RequireWaifuDuelChannelTests
    {
        private readonly RequireWaifuDuelChannel _preconditionAttribute;
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly Mock<ICommandContext> _landManagerMock = new(MockBehavior.Strict);


        public RequireWaifuDuelChannelTests()
        {
            _preconditionAttribute = new();
        }

        [TestMethod]
        public async Task Should_Return_Success()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, serviceProvider);
        }

        [TestMethod]
        public async Task Should_Return_Error_Only_Server()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var result = await _preconditionAttribute.CheckPermissionsAsync(_commandContextMock.Object, null, serviceProvider);
        }
    }
}
