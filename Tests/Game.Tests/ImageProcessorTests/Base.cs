using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Services;
using Sanakan.ShindenApi;
using Sanakan.Game.Services;
using Shinden.API;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Sanakan.Game.Services.Abstractions;

namespace Sanakan.Game.Tests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly IImageProcessor _imageProcessor;
        protected readonly Mock<IShindenClient> _shindenClientMock = new(MockBehavior.Strict);
        protected readonly Mock<IFileSystem> _fileSystemMock = new(MockBehavior.Strict);
        public Base()
        {
            var httpClient = new HttpClient();

            _imageProcessor = new ImageProcessor(
                _shindenClientMock.Object,
                _fileSystemMock.Object,
                httpClient);
        }
    }
}
