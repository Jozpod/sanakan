using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.Tests.Shared;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.SendAndGetSafariImageUrlAsync(SafariImage, IMessageChannel)"/> method.
    /// </summary>
    [TestClass]
    public class SendAndGetSafariImageUrlAsyncTest : Base
    {
        [TestMethod]
        public async Task Should_Return_Url()
        {
            var safariImage = new SafariImage();
            var expectedUrl = "https://test.com/image.png";
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var attachmentMock = new Mock<IAttachment>(MockBehavior.Strict);
            var attachments = new List<IAttachment> { attachmentMock.Object };

            _fileSystemMock
                .Setup(pr => pr.Exists(It.IsAny<string>()))
                .Returns(false);

            _fileSystemMock
                .Setup(pr => pr.OpenRead(It.IsAny<string>()))
                .Returns(new MemoryStream());

            messageChannelMock.SetupSendFileAsync(userMessageMock.Object);

            userMessageMock
               .Setup(pr => pr.Attachments)
               .Returns(attachments);

            attachmentMock
                .Setup(pr => pr.Url)
                .Returns(expectedUrl);

            var actualUrl = await _waifuService.SendAndGetSafariImageUrlAsync(safariImage, messageChannelMock.Object);
            actualUrl.Should().Be(expectedUrl);
        }
    }
}
