using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sanakan.Game.Tests
{
    /// <summary>
    /// Defines tests for <see cref="ExperienceUtils"/> class.
    /// </summary>
    [TestClass]
    public class ExperienceUtilsTests
    {
        [TestMethod]
        [DataRow(1uL, 9uL)]
        [DataRow(2uL, 33uL)]
        [DataRow(3uL, 74uL)]
        [DataRow(4uL, 131uL)]
        public void Should_Calculate_Experience_For_Given_Levels(ulong level, ulong expected)
        {
            var result = ExperienceUtils.CalculateExpForLevel(level);
            result.Should().Be(expected);
        }

        [TestMethod]
        [DataRow(100uL, 3uL)]
        [DataRow(2000uL, 15uL)]
        [DataRow(30000uL, 60uL)]
        [DataRow(400000uL, 221uL)]
        public void Should_Calculate_Level(ulong experience, ulong expected)
        {
            var result = ExperienceUtils.CalculateLevel(experience);
            result.Should().Be(expected);
        }
    }
}
