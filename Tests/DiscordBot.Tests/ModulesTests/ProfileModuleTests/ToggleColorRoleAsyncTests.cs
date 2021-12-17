using DiscordBot.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ToggleColorRoleAsync(FColor, SCurrency)"/> method.
    /// </summary>
    [TestClass]
    public class ToggleColorRoleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Toggle_Color_Role()
        {
            var utcNow = DateTime.UtcNow;
            var color = FColor.AgainBlue;
            var currency = SCurrency.Tc;
            var user = new User(1ul, utcNow);
            user.TcCount = 39999;
            var guildOptions = new GuildOptions(1ul, 50);
            guildOptions.AdminRoleId = 1ul;

            _userMock
                .Setup(pr => pr.Id)
                .Returns(user.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _guildMock
                .Setup(pr => pr.Id)
                .Returns(guildOptions.Id);

            _userRepositoryMock
                .Setup(pr => pr.GetUserOrCreateAsync(user.Id))
                .ReturnsAsync(user);

            _guildConfigRepositoryMock
                .Setup(pr => pr.GetCachedGuildFullConfigAsync(guildOptions.Id))
                .ReturnsAsync(guildOptions);

            _profileServiceMock
                .Setup(pr => pr.HasSameColor(_guildUserMock.Object, color))
                .Returns(false);

            _profileServiceMock
                .Setup(pr => pr.RemoveUserColorAsync(_guildUserMock.Object, FColor.None))
                .Returns(Task.CompletedTask);

            _profileServiceMock
               .Setup(pr => pr.SetUserColorAsync(
                   _guildUserMock.Object,
                   guildOptions.AdminRoleId.Value, color))
               .ReturnsAsync(true);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _userRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.ToggleColorRoleAsync(color, currency);
        }
    }
}
