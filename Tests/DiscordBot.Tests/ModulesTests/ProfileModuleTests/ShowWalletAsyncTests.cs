using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ProfileModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ProfileModule.ShowWalletAsync(IUser?)"/> method.
    /// </summary>
    [TestClass]
    public class ShowWalletAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_Containing_Wallet_Info()
        {
            var databaseUser = new User(1ul, DateTime.UtcNow);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(databaseUser.Id);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("mention");

            _userRepositoryMock
                .Setup(pr => pr.GetCachedAsync(databaseUser.Id))
                .ReturnsAsync(databaseUser);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.ShowWalletAsync();
        }
    }
}
