using DiscordBot.Services.PocketWaifu.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.Services.Executor;
using Sanakan.ShindenApi;
using Sanakan.Web.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests
{
    [TestClass]
    public class GetUsersOwningCharacterCardAsyncTests
    {
        private readonly WaifuController _controller;
        private readonly Mock<IShindenClient> _shindenClientMock;
        private readonly Mock<IWaifuService> _waifuServiceMock;
        private readonly Mock<IExecutor> _executorMock;
        private readonly Mock<IFileSystem> _fileSystemMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICardRepository> _cardRepositoryMock;
        private readonly Mock<IUserContext> _userContextMock;
        private readonly Mock<ICacheManager> _cacheManagerMock;
        private readonly Mock<IJwtBuilder> _jwtBuilderMock;

        public GetUsersOwningCharacterCardAsyncTests()
        {
            _controller = new WaifuController(
                _shindenClientMock.Object,
                _waifuServiceMock.Object,
                _executorMock.Object,
                _fileSystemMock.Object,
                _userRepositoryMock.Object,
                _cardRepositoryMock.Object,
                _userContextMock.Object,
                _cacheManagerMock.Object,
                _jwtBuilderMock.Object);
        }

        [TestMethod]
        public async Task Should_Return_NotFound()
        {
            var userId = 0ul;
            var expected = new[] { 0ul };

            _userRepositoryMock
                .Setup(pr => pr.GetUserShindenIdsByHavingCharacterAsync(userId))
                .ReturnsAsync(expected);

            var result = await _controller.GetUsersOwningCharacterCardAsync(userId);
        }

        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var userId = 0ul;
            var expected = Enumerable.Empty<ulong>();

            _userRepositoryMock
                .Setup(pr => pr.GetUserShindenIdsByHavingCharacterAsync(userId))
                .ReturnsAsync(expected);

            var result = await _controller.GetUsersOwningCharacterCardAsync(userId);
        }
    }
}
