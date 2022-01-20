using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.Tests.Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetWaifuProfileImageUrlAsync(Card, IMessageChannel)"/> method.
    /// </summary>
    [TestClass]
    public class GetWaifuProfileImageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Image_Url()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            var attachmentUrl = "https://www.test.com/image.png";
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var attachmentMock = new Mock<IAttachment>(MockBehavior.Strict);
            var fakeIamge = new Image<Rgba32>(300, 300);
            var attachments = new List<IAttachment>
            {
                attachmentMock.Object,
            };

            _imageProcessorMock
                .Setup(pr => pr.GetWaifuCardImageAsync(It.IsAny<Card>()))
                .ReturnsAsync(fakeIamge);

            _imageProcessorMock
               .Setup(pr => pr.GetWaifuInProfileCardAsync(It.IsAny<Card>()))
               .ReturnsAsync(fakeIamge);

            _fileSystemMock
                .Setup(pr => pr.OpenWrite(It.IsAny<string>()))
                .Returns(() => new MemoryStream());

            messageChannelMock.SetupSendFileAsync(userMessageMock.Object);

            userMessageMock
                .Setup(pr => pr.Attachments)
                .Returns(attachments);

            attachmentMock
                .Setup(pr => pr.Url)
                .Returns(attachmentUrl);

            var iamgeUrl = await _waifuService.GetWaifuProfileImageUrlAsync(card, messageChannelMock.Object);
            iamgeUrl.Should().NotBeNull();
        }
    }
}
