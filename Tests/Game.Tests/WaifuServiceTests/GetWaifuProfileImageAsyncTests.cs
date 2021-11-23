using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using Discord;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    [TestClass]
    public class GetWaifuProfileImageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Embed()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);
            var messageChannelMock = new Mock<IMessageChannel>(MockBehavior.Strict);

            var embed = await _waifuService.GetWaifuProfileImageAsync(card, messageChannelMock.Object);
            embed.Should().NotBeNull();

            messageChannelMock.Verify();
        }
    }
}
