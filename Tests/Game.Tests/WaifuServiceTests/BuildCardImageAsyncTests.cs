using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.BuildCardImageAsync(Card, ITextChannel, IUser, bool)"/> method.
    /// </summary>
    [TestClass]
    public class BuildCardImageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Embed()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            var channelMock = new Mock<ITextChannel>(MockBehavior.Strict);
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

            _fileSystemMock
                .Setup(pr => pr.OpenRead(It.IsAny<string>()))
                .Returns(new MemoryStream());

            channelMock
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
                .Returns("url");

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            var embed = await _waifuService.BuildCardImageAsync(
                card, channelMock.Object, guildUserMock.Object, true);
            embed.Should().NotBeNull();
            embed.Description.Should().NotBeNullOrEmpty();
        }
    }
}
