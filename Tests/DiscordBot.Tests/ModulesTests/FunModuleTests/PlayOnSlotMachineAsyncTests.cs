using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.PlayOnSlotMachineAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class PlayOnSlotMachineAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _slotMachineMock
                .Setup(pr => pr.ToPay(user.SMConfig))
                .Returns(100);

            _slotMachineMock
                .Setup(pr => pr.Draw(user))
                .Returns("test");

            _slotMachineMock
                .Setup(pr => pr.Play(user))
                .Returns(100);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(2))
                .Returns(1);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNull();
            });

            await _module.PlayOnSlotMachineAsync();
        }
    }
}
