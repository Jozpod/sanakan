using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.Modules;
using Sanakan.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.Test
{
    [TestClass]
    public class LandsModuleTests
    {
        private readonly LandsModule _module;
        private readonly Mock<LandManager> _landManagerMock;
        private readonly Mock<IAllRepository> _repositoryMock;

        public LandsModuleTests()
        {
            _module = new LandsModule(
                _landManagerMock.Object,
                _repositoryMock.Object);
        }

        [TestMethod]
        public async Task Should_Tell_When_User_Does_Not_Own_Land()
        {

            _repositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(It.IsAny<ulong>()));

            _landManagerMock
                .Setup(pr => pr.DetermineLand(
                    It.IsAny<IEnumerable<MyLand>>(),
                    It.IsAny<SocketGuildUser>(),
                    It.IsAny<string>()
                ));

            await _module.ShowPeopleAsync();
        }
    }
}
