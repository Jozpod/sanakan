using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.GetAttactAfterLevelUp(Rarity, int)"/> method.
    /// </summary>
    [TestClass]
    public class GetAttactAfterLevelUpTests : Base
    {
        [TestMethod]
        public void Should_Return_Value()
        {
            var rarity = Rarity.A;
            var attack = 100;

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(105, 107))
                .Returns(106);

            var value = _waifuService.GetAttactAfterLevelUp(rarity, attack);
            value.Should().BeGreaterThan(95);
        }
    }
}
