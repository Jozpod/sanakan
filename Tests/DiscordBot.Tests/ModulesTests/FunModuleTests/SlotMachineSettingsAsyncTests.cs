using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using Sanakan.Game.Models;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.SlotMachineSettingsAsync"/> method.
    /// </summary>
    [TestClass]
    public class SlotMachineSettingsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Message_Containing_Info()
        {
            var utcNow = DateTime.UtcNow;

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNull();
            });

            await _module.SlotMachineSettingsAsync();
        }

        [TestMethod]
        [DataRow(SlotMachineSetting.Beat, "10")]
        [DataRow(SlotMachineSetting.Info, "1")]
        [DataRow(SlotMachineSetting.Multiplier, "3")]
        [DataRow(SlotMachineSetting.Rows, "3")]
        public async Task Should_Set_Machine_And_Send_Confirm_Message(SlotMachineSetting slotMachineSetting, string value)
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);

            _guildUserMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _guildUserMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

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

            await _module.SlotMachineSettingsAsync(slotMachineSetting, value);
        }
    }
}