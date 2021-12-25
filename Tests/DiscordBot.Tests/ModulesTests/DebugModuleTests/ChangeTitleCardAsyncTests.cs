using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.ChangeTitleCardAsync(ulong, string?)"/> method.
    /// </summary>
    [TestClass]
    public class ChangeTitleCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Change_Card_Title_And_Send_Confirm_Message()
        {
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.B, Dere.Bodere, DateTime.UtcNow);

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdAsync(card.Id))
                .ReturnsAsync(card);

            _cardRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.ChangeTitleCardAsync(card.Id, "new title");
        }
    }
}
