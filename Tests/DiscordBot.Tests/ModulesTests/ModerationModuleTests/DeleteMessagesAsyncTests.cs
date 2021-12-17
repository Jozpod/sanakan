using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.DeleteMessagesAsync(int)"/> method.
    /// </summary>
    [TestClass]
    public class DeleteMessagesAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Invalid_Duration()
        {
            var count = 10;
            var messages = new List<IMessage> { new Mock<IMessage>().Object };
            var messagesBatch = Enumerable.Repeat(messages, count)
                .ToAsyncEnumerable();

            _contextMessageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            _textChannelMock
                .Setup(pr => pr.GetMessagesAsync(count, CacheMode.AllowDownload, null))
                .Returns(messagesBatch);

            _textChannelMock
                .Setup(pr => pr.DeleteMessagesAsync(messages, null))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.DeleteMessagesAsync(count);
        }
    }
}
