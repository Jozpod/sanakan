using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Moq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using System;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.GetOneFromManyAsync(string[])"/> method.
    /// </summary>
    [TestClass]
    public class GetOneFromManyAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message()
        {
            var options = new[] { "one", "two", "three" };

            _randomNumberGeneratorMock
                .Setup(pr => pr.Shuffle(It.IsAny<IEnumerable<string>>()))
                .Returns(options);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(100, 500))
                .Returns(100);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<string>>()))
                .Returns<IEnumerable<string>>(pr => pr.First());

            _taskManagerMock
                .Setup(pr => pr.Delay(It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            _guildUserMock
                .Setup(pr => pr.Nickname)
                .Returns("nickname");

            _guildUserMock
                .Setup(pr => pr.GetAvatarUrl(Discord.ImageFormat.Auto, 128))
                .Returns("https://test.com/avatar.png");

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.GetOneFromManyAsync(options);
        }
    }
}
