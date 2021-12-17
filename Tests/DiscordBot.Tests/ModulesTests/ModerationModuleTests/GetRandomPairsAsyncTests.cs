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
    /// Defines tests for <see cref="ModerationModule.GetRandomPairsAsync(uint)"/> method.
    /// </summary>
    [TestClass]
    public class GetRandomPairsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Number_Pairs()
        {
            var count = 3u;

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<int>>()))
                .Returns<IEnumerable<int>>((items) => items.First());

            SetupSendMessage((message, embed) =>
            {
                embed.Description.Should().NotBeNull();
            });

            await _module.GetRandomPairsAsync(count);
        }
    }
}
