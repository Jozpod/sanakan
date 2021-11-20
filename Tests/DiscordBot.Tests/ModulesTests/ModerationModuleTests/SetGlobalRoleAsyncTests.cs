using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    [TestClass]
    public class SetGlobalRoleAsyncTests : Base
    {
        private readonly Mock<IRole> _roleMock = new(MockBehavior.Strict);

        [TestMethod]
        public async Task Should_Send_Message()
        {
            _helperServiceMock
                .Setup(pr => pr.GivePrivateHelp("Moderacja"))
                .Returns("test info");
            
            await _module.SetGlobalRoleAsync(_roleMock.Object);
        }
    }
}
