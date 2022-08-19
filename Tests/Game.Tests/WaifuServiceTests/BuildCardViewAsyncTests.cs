using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using Sanakan.Tests.Shared;
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
               .Setup(pr => pr.OpenRead(It.IsAny<string>()))
               .Returns(new MemoryStream());

            _fileSystemMock
                 .Setup(pr => pr.Exists(It.IsAny<string>()))
                 .Returns(true);

            textChannelMock.SetupSendFileAsync(userMessageMock.Object);

            userMessageMock
                .Setup(pr => pr.Attachments)
                .Returns(attachments);

            attachmentMock
                .Setup(pr => pr.Url)
                .Returns("https://test.com/image.png");

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            var embed = await _waifuService.BuildCardViewAsync(card, textChannelMock.Object, guildUserMock.Object);
            embed.Should().NotBeNull();
            embed.Description.Should().NotBeNullOrEmpty();
        }
    }
}
