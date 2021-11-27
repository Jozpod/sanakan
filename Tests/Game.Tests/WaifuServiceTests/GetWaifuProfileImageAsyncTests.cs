using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using Discord;
using Sanakan.Game.Services.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

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
                .Returns(new MemoryStream());

            messageChannelMock
                .Setup(pr => pr.SendFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<bool>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(userMessageMock.Object)
                .Verifiable();

            userMessageMock
                .Setup(pr => pr.Attachments)
                .Returns(attachments);

            attachmentMock
                .Setup(pr => pr.Url)
                .Returns(attachmentUrl);

            var iamgeUrl = await _waifuService.GetWaifuProfileImageUrlAsync(card, messageChannelMock.Object);
            iamgeUrl.Should().NotBeNull();

            messageChannelMock.Verify();
        }
    }
}
