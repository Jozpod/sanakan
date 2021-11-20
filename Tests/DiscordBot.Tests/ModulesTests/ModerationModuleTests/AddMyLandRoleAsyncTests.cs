using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    [TestClass]
    public class AddMyLandRoleAsyncTests : Base
    {
        private readonly Mock<IRole> _roleMock = new(MockBehavior.Strict);

        [TestMethod]
        public async Task Should_Add_Land_Role_And_Send_Message()
        {
            _roleMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            await _module.AddMyLandRoleAsync(_roleMock.Object, _roleMock.Object);
        }
    }
}
