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
        [DataRow(1, 1)]
        [DataRow(2, 1)]
        [DataRow(3, 1)]
        [DataRow(4, 1)]
        public void Should_Calculate_Experience_For_Given_Levels(ulong level, ulong expected)
        {
            var result = ExperienceUtils.CalculateExpForLevel(level);
            result.Should().Be(expected);
        }

        [TestMethod]
        [DataRow(100, 1)]
        [DataRow(2000, 1)]
        [DataRow(30000, 1)]
        [DataRow(400000, 1)]
        public void Should_Calculate_Level(ulong experience, ulong expected)
        {
            var result = ExperienceUtils.CalculateLevel(experience);
            result.Should().Be(expected);
        }
    }
}
