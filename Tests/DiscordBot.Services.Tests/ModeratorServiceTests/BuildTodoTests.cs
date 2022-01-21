using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Services.Abstractions;
using System.Collections.Generic;

namespace DiscordBot.Services.Tests.ModeratorServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IModeratorService.BuildTodo(IMessage, IGuildUser)"/> method.
    /// </summary>
    [TestClass]
    public class BuildTodoTests : Base
    {
        [TestMethod]
        public void Should_Build_ToDo()
        {
            var messageMock = new Mock<IMessage>(MockBehavior.Strict);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var attachmentMock = new Mock<IAttachment>(MockBehavior.Strict);
            var attachments = new List<IAttachment> { attachmentMock.Object };
            var url = "https://test.com/image.png";

            attachmentMock
                .Setup(pr => pr.Url)
                .Returns(url);

            messageMock
                .Setup(pr => pr.Attachments)
                .Returns(attachments);

            messageMock
                .Setup(pr => pr.Author)
                .Returns(guildUserMock.Object);

            guildUserMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns("https://test.com/image.png");

            guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            messageMock
                .Setup(pr => pr.Content)
                .Returns("content");

            var embed = _moderatorService.BuildTodo(messageMock.Object, guildUserMock.Object);
            embed.Should().NotBeNull();
        }
    }
}
