using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
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
    /// Defines tests for <see cref="IWaifuService.BuildCardViewAsync(Card, ITextChannel, IUser)"/> method.
    /// </summary>
    [TestClass]
    public class BuildCardViewAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Embed_Image_With_Stats()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            var textChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var showStats = true;
            var attachment = new Mock<IAttachment>(MockBehavior.Strict);
            var attachments = new List<IAttachment>() { attachment.Object };
            var utcNow = DateTime.UtcNow;
            var imageUrl = "https://www.test.com/image.png";
            var guildUserMock = userMock.As<IGuildUser>();

            _fileSystemMock
                .Setup(pr => pr.Exists(It.IsAny<string>()))
                .Returns(true);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _fileSystemMock
                .Setup(pr => pr.GetCreationTime(It.IsAny<string>()))
                .Returns(utcNow);

            textChannelMock
                 .Setup(pr => pr.SendFileAsync(
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

            attachment
                .Setup(pr => pr.Url)
                .Returns(imageUrl);

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            var embed = await _waifuService.BuildCardImageAsync(card, textChannelMock.Object, userMock.Object, showStats);
            embed.Should().NotBeNull();
            embed.Description.Should().NotBeNullOrEmpty();
            embed.Image!.Value.Url.Should().Be(imageUrl);
        }

        [TestMethod]
        public async Task Should_Return_Embed_Image_With_No_Stats()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            var textChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var showStats = false;
            var attachment = new Mock<IAttachment>(MockBehavior.Strict);
            var attachments = new List<IAttachment>() { attachment.Object };
            var utcNow = DateTime.UtcNow;
            var imageUrl = "https://www.test.com/image.png";
            var guildUserMock = userMock.As<IGuildUser>();
            var fakeImage = new Image<Rgba32>(400, 400);

            _imageProcessorMock
                .Setup(pr => pr.GetWaifuCardImageAsync(card))
                .ReturnsAsync(fakeImage);

            _imageProcessorMock
                .Setup(pr => pr.GetWaifuInProfileCardAsync(card))
                .ReturnsAsync(fakeImage);

            _fileSystemMock
                .Setup(pr => pr.OpenWrite(It.IsAny<string>()))
                .Returns(() => new MemoryStream());

            _fileSystemMock
                .Setup(pr => pr.Exists(It.IsAny<string>()))
                .Returns(true);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _fileSystemMock
                .Setup(pr => pr.GetCreationTime(It.IsAny<string>()))
                .Returns(utcNow);

            textChannelMock
                 .Setup(pr => pr.SendFileAsync(
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

            attachment
                .Setup(pr => pr.Url)
                .Returns(imageUrl);

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            var embed = await _waifuService.BuildCardImageAsync(card, textChannelMock.Object, userMock.Object, showStats);
            embed.Should().NotBeNull();
            embed.Description.Should().NotBeNullOrEmpty();
            embed.Image!.Value.Url.Should().Be(imageUrl);
        }
    }
}
