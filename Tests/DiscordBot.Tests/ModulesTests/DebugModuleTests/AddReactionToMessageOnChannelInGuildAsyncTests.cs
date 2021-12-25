using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.AddReactionToMessageOnChannelInGuildAsync(ulong, ulong, ulong, string)"/> method.
    /// </summary>
    [TestClass]
    public class AddReactionToMessageOnChannelInGuildAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Add_Reaction_To_Message()
        {
            var guildId = 1ul;
            var channelId = 1ul;
            var messageId = 1ul;
            var reaction = ":white_check_mark:";
            var guildChannelMock = new Mock<IGuildChannel>();
            var messageChannelMock = guildChannelMock.As<IMessageChannel>();

            _discordClientMock
                .Setup(pr => pr.GetGuildAsync(guildId, CacheMode.AllowDownload, null))
                .ReturnsAsync(_guildMock.Object);

            _guildMock
               .Setup(pr => pr.GetChannelAsync(channelId, CacheMode.AllowDownload, null))
               .ReturnsAsync(guildChannelMock.Object);

            messageChannelMock
               .Setup(pr => pr.GetMessageAsync(messageId, CacheMode.AllowDownload, null))
               .ReturnsAsync(_userMessageMock.Object);

            _userMessageMock
                .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
               .Returns(Task.CompletedTask);
            
            await _module.AddReactionToMessageOnChannelInGuildAsync(guildId, channelId, messageId, reaction);
        }
    }
}
