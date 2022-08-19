using Microsoft.Extensions.Options;
using Moq;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Game.Services;
using Sanakan.Game.Services.Abstractions;

namespace Sanakan.Game.Tests
{
    public abstract class Base
    {
        protected static IImageProcessor _imageProcessor;
        protected static Mock<IImageResolver> _imageResolverMock = new(MockBehavior.Strict);
        protected static Mock<IOptionsMonitor<ImagingConfiguration>> _imagingConfigurationMock = new(MockBehavior.Strict);
        protected static Mock<IResourceManager> _resourceManagerMock = new(MockBehavior.Strict);
        protected static Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);
        private static readonly ImagingConfiguration _imagingConfiguration;

        static Base()
        {
            _imagingConfiguration = new ImagingConfiguration
            {
                CharacterImageWidth = 600,
                CharacterImageHeight = 600,
                StatsImageHeight = 100,
                StatsImageWidth = 100,
                ProfileImageWidth = 300,
                ProfileImageHeight = 270,
            };

            _imagingConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(_imagingConfiguration);

            var assembly = typeof(IImageProcessor).Assembly;

            _resourceManagerMock
                .Setup(pr => pr.GetResourceStream(It.IsAny<string>()))
                .Returns(() => assembly.GetManifestResourceStream("Sanakan.Game.Fonts.Digital.ttf")!);

            _imageProcessor = new ImageProcessor(
                _imagingConfigurationMock.Object,
                _resourceManagerMock.Object,
                _fileSystemMock.Object,
                _imageResolverMock.Object);
        }
    }
}
