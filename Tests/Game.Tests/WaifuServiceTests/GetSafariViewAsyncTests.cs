using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetSafariViewAsync(Models.SafariImage, Card, Discord.IMessageChannel)"/> method.
    /// </summary>
    [TestClass]
    public class GetSafariViewAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Url()
        {
            var safariImage = new SafariImage();
            var card = new Card(1ul, "Test Card", "Test Card", 100, 50, Rarity.A, Dere.Bodere, DateTime.UtcNow);
            var image = new Image<Rgba32>(300, 300);
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

            _imageProcessorMock
                .Setup(pr => pr.GetWaifuCardImageAsync(card))
                .ReturnsAsync(image);

            _imageProcessorMock
                .Setup(pr => pr.GetCatchThatWaifuImage(
                    It.IsAny<Image<Rgba32>>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .Returns(image);

            messageChannelMock
                .Setup(pr => pr.SendFileAsync(
                    It.IsAny<Stream>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<bool>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(userMessageMock.Object);

            userMessageMock
               .Setup(pr => pr.Attachments)
               .Returns(attachments);

            attachmentMock
                .Setup(pr => pr.Url)
                .Returns(expectedUrl);

            var actualUrl = await _waifuService.GetSafariViewAsync(safariImage, card, messageChannelMock.Object);
            actualUrl.Should().Be(expectedUrl);
        }
    }
}
