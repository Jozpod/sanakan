using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Game.Services.Abstractions;
using Sanakan.ShindenApi;

namespace DiscordBot.ServicesTests.ProfileServiceTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly IProfileService _profileService;
        protected readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        protected readonly Mock<IImageProcessor> _imageProcessorMock = new(MockBehavior.Strict);
        protected readonly Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);
  
        public Base()
        {
            _profileService = new ProfileService(
                _shindenClientMock.Object,
                _imageProcessorMock.Object,
                _fileSystemMock.Object,
                NullLogger<ProfileService>.Instance);
        }
    }
}
