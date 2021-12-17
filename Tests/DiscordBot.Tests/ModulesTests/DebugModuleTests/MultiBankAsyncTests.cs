using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.MultiBankAsync(IGuildUser[])"/> method.
    /// </summary>
    [TestClass]
    public class MultiBankAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Ban_Users_And_Send_Confirm_Message()
        {
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);

            _guildMock
                .Setup(pr => pr.AddBanAsync(guildUserMock.Object, 0, It.IsAny<string>(), null))
                .Returns(Task.CompletedTask);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.MultiBanAsync(guildUserMock.Object);
        }
    }
}
