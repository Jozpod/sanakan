using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ModerationModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ModerationModule.GetRandomUserAsync(TimeSpan)"/> method.
    /// </summary>
    [TestClass]
    public class GetRandomUserAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_With_Random_User()
        {
            var utcNow = DateTime.UtcNow;
            var duration = TimeSpan.FromSeconds(10);
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var users = new List<IUser>() { guildUserMock.Object };
            var usersBatch = new[]
            {
                users,
            }.ToAsyncEnumerable();

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _userMessageMock
                .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                .Returns(Task.CompletedTask);

            _taskManagerMock
                .Setup(pr => pr.Delay(It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            _userMessageMock
                .Setup(pr => pr.RemoveReactionAsync(It.IsAny<IEmote>(), It.IsAny<IUser>(), null))
                .Returns(Task.CompletedTask);

            _userMessageMock
                .Setup(pr => pr.GetReactionUsersAsync(It.IsAny<IEmote>(), 300, null))
                .Returns(usersBatch);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<IUser>>()))
                .Returns<IEnumerable<IUser>>(items => items.First());

            guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userMessageMock
                .Setup(pr => pr.DeleteAsync(null))
                .Returns(Task.CompletedTask);

            SetupSendMessage();

            await _module.GetRandomUserAsync(duration);
        }
    }
}
