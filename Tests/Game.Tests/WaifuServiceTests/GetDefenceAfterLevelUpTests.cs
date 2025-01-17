﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetDefenceAfterLevelUp(Rarity, int)"/> method.
    /// </summary>
    [TestClass]
    public class GetDefenceAfterLevelUpTests : Base
    {
        [TestMethod]
        public void Should_Return_Value()
        {
            var rarity = Rarity.S;
            var defence = 200;

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(245, 247))
                .Returns(246);

            var value = _waifuService.GetDefenceAfterLevelUp(rarity, defence);
            value.Should().BeGreaterThan(90);
        }
    }
}
