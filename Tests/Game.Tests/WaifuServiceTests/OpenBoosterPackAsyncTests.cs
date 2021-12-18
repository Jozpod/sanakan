using Discord;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.OpenBoosterPackAsync(ulong?, BoosterPack)"/> method.
    /// </summary>
    [TestClass]
    public class OpenBoosterPackAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Cards()
        {
            var discordUserId = 1ul;
            var boosterPack = new BoosterPack
            {

            };

            var cards = await _waifuService.OpenBoosterPackAsync(discordUserId, boosterPack);
            cards.Should().NotBeNull();
        }
    }
}
