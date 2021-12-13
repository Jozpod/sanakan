using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using FluentAssertions;
using Sanakan.DiscordBot.Services.Abstractions;
using System.Collections.Generic;
using System;

namespace DiscordBot.ServicesTests.HelperServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IHelperService.BuildRaportInfo(IMessage, string, string, ulong)"/> method.
    /// </summary>
    [TestClass]
    public class BuildRaportInfoTests : Base
    {
        [TestMethod]
        public void Should_Return_Embed_With_Report_Info()
        {
            var messageMock = new Mock<IMessage>(MockBehavior.Strict);
            var reportAuthor = "test report author";
            var reason = "test report author";
            var reportId = 1ul;
            var createdAt = DateTime.UtcNow;
            var attachmentUrl = "https://www.test.com/file.png";
            var content = "messge content";
            var username = "username";
            var avatarUrl = "https://www.test.com/avatar.png";
            var attachmentMock = new Mock<IAttachment>(MockBehavior.Strict);
            var userMock = new Mock<IUser>(MockBehavior.Strict);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);
            var attachments = new List<IAttachment>
            {
                attachmentMock.Object
            };

            attachmentMock
               .Setup(pr => pr.Url)
               .Returns(attachmentUrl);

            messageMock
                .Setup(pr => pr.Attachments)
                .Returns(attachments);

            messageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            messageChannelMock
                .Setup(pr => pr.Name)
                .Returns("test channel");

            messageMock
               .Setup(pr => pr.Author)
               .Returns(userMock.Object);

            userMock
                .Setup(pr => pr.Username)
                .Returns(username);

            userMock
                .Setup(pr => pr.GetAvatarUrl(ImageFormat.Auto, 128))
                .Returns(avatarUrl);

            messageMock
                .Setup(pr => pr.Content)
                .Returns(content);

            messageMock
               .Setup(pr => pr.CreatedAt)
               .Returns(createdAt);

            var result = _helperService.BuildRaportInfo(messageMock.Object, reportAuthor, reason, reportId);
            result.Should().NotBeNull();
            result.Fields.Should().HaveCount(5);

            foreach (var field in result.Fields)
            {
                field.Name.Should().NotBeNullOrEmpty();
                field.Value.Should().NotBeNullOrEmpty();
            }
        }
    }
}
