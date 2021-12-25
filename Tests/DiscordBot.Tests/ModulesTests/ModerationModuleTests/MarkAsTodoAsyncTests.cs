using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.MarkAsTodoAsync(ulong, string)"/> method.
    /// </summary>
    [TestClass]
    public class MarkAsTodoAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Mark_As_ToDo_And_Send_Confirm_Message()
        {
            var messageId = 1ul;
            var serverName = "test server";
            var guilds = new List<IGuild> { _guildMock.Object };
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var userId = 1ul;
            var guildOptions = new GuildOptions(1ul, 50ul);
            var guildChannelMock = new Mock<IGuildChannel>(MockBehavior.Strict);
            var textChannelMock = guildChannelMock.As<ITextChannel>();

            _discordClientMock
                .Setup(pr => pr.GetGuildsAsync(CacheMode.AllowDownload, null))
                .ReturnsAsync(guilds);

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _guildMock
                .Setup(pr => pr.Name)
                .Returns(serverName);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            _guildMock
                .Setup(pr => pr.GetUserAsync(userId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildUserMock.Object);

            guildUserMock
                .Setup(pr => pr.GuildPermissions)
                .Returns(new GuildPermissions(administrator: true));

            _guildConfigRepositoryMock
                 .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOptions.Id))
                 .ReturnsAsync(guildOptions);

            _guildMock
                .Setup(pr => pr.GetChannelAsync(guildOptions.ToDoChannelId, CacheMode.AllowDownload, null))
                .ReturnsAsync(guildChannelMock.Object);

            _contextMessageMock
                .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                .Returns(Task.CompletedTask);
            
            _moderatorServiceMock
                .Setup(pr => pr.BuildTodo(_userMessageMock.Object, _guildUserMock.Object))
                .Returns(new EmbedBuilder().Build());

            _messageChannelMock
                .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_userMessageMock.Object);

            _userMessageMock
                .Setup(pr => pr.Channel)
                .Returns(textChannelMock.Object);

            textChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(_userMessageMock.Object);

            textChannelMock
                .Setup(pr => pr.GuildId)
                .Returns(guildOptions.Id);

            textChannelMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMessageMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.MarkAsTodoAsync(messageId, serverName);
        }
    }
}
