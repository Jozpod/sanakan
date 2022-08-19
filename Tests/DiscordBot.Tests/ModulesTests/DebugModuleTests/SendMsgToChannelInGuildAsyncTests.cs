using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using Sanakan.Tests.Shared;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.SendMsgToChannelInGuildAsync(ulong, ulong, string)"/> method.
    /// </summary>
    [TestClass]
    public class SendMsgToChannelInGuildAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_To_Given_Channel()
        {
            var guildId = 1ul;
            var channelId = 1ul;
            var message = "message";
            var guildChannelMock = new Mock<IGuildChannel>(MockBehavior.Strict);
            var messageChannelMock = guildChannelMock.As<IMessageChannel>();

            _discordClientMock
               .Setup(pr => pr.GetGuildAsync(guildId, CacheMode.AllowDownload, null))
               .ReturnsAsync(_guildMock.Object);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(channelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildChannelMock.Object);

            messageChannelMock.SetupSendMessageAsync(_userMessageMock.Object);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(messageChannelMock.Object);

            SetupSendMessage();

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.SendMsgToChannelInGuildAsync(guildId, channelId, message);
        }
    }
}
