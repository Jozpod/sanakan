using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using Sanakan.ShindenApi;
using System.Collections.Generic;
using Sanakan.ShindenApi.Models;
using FluentAssertions;
using System;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenModule.ShowNewEpisodesAsync"/> method.
    /// </summary>
    [TestClass]
    public class ShowNewEpisodesAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_New_Epsiodes()
        {
            var dmChannelMock = new Mock<IDMChannel>(MockBehavior.Strict);
            var epsiodesResult = new Result<List<NewEpisode>>()
            {
                Value = new List<NewEpisode>
                {
                    new NewEpisode
                    {
                    }
                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetNewEpisodesAsync())
                .ReturnsAsync(epsiodesResult);

            _userMock
                .Setup(pr => pr.GetOrCreateDMChannelAsync(null))
                .ReturnsAsync(dmChannelMock.Object);

            dmChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(_userMessageMock.Object);

            _taskManagerMock
                .Setup(pr => pr.Delay(It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            dmChannelMock
                .Setup(pr => pr.CloseAsync(null))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            _userMessageMock
                .Setup(pr => pr.ModifyAsync(It.IsAny<Action<MessageProperties>>(), null))
                .Returns(Task.CompletedTask);

            await _module.ShowNewEpisodesAsync();
        }
    }
}
