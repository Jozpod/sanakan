using Discord.WebSocket;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.Common;
using Sanakan.Game.Services;

namespace DiscordBot.ServicesTests.ProfileServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly ProfileService _profileService;
        protected readonly Mock<DiscordSocketClient> _discordSocketClientMock = new(MockBehavior.Strict);
        protected readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        protected readonly Mock<IImageProcessor> _imageProcessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);
  
        public Base()
        {
            var serviceCollection = new ServiceCollection();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _profileService = new(
                _discordSocketClientMock.Object,
                _shindenClientMock.Object,
                _imageProcessorMock.Object,
                _fileSystemMock.Object,
                serviceScopeFactory,
                NullLogger<ProfileService>.Instance);
        }
    }
}
