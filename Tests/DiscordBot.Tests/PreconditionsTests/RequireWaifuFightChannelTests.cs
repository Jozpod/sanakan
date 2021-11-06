using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Discord.Rest;
using System.IO;
using Sanakan.Preconditions;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Test
{
    [TestClass]
    public class RequireWaifuFightChannelTests
    {
        private readonly RequireWaifuFightChannel _preconditionAttribute;
        private readonly Mock<ICommandContext> _commandContextMock = new(MockBehavior.Strict);
        private readonly Mock<ICommandContext> _landManagerMock = new(MockBehavior.Strict);


        public RequireWaifuFightChannelTests()
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
