using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var epsiodesResult = new ShindenResult<List<NewEpisode>>()
            {
                Value = new List<NewEpisode>
                {
                    new NewEpisode
                    {
                        EpisodeLength = TimeSpan.FromMinutes(20),
                        AddDate = DateTime.UtcNow,
                    }
                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetNewEpisodesAsync())
                .ReturnsAsync(epsiodesResult);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userMock
                .Setup(pr => pr.CreateDMChannelAsync(null))
                .ReturnsAsync(dmChannelMock.Object);

            dmChannelMock.SetupSendMessageAsync(_userMessageMock.Object);

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
