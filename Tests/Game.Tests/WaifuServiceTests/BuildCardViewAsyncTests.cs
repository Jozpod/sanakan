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
        public async Task Should_Return_Embed_Image()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            var textChannelMock = new Mock<ITextChannel>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var userMessageMock = new Mock<IUserMessage>(MockBehavior.Strict);
            var attachmentMock = new Mock<IAttachment>(MockBehavior.Strict);
            var attachments = new List<IAttachment>
            {
                attachmentMock.Object,
            };

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(DateTime.UtcNow);

            _fileSystemMock
                .Setup(pr => pr.GetCreationTime(It.IsAny<string>()))
                .Returns(DateTime.UtcNow);

            _fileSystemMock
                 .Setup(pr => pr.Exists(It.IsAny<string>()))
                 .Returns(true);

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

            attachmentMock
                .Setup(pr => pr.Url)
                .Returns("url");

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            var embed = await _waifuService.BuildCardViewAsync(card, textChannelMock.Object, guildUserMock.Object);
            embed.Should().NotBeNull();
            embed.Description.Should().NotBeNullOrEmpty();
        }
    }
}
