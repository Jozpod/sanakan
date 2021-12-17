using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common.Configuration;
using Sanakan.Extensions;
using System.Linq;

namespace Sanakan.Common.Tests
{
    /// <summary>
    /// Defines tests for <see cref="StringExtension"/> class.
    /// </summary>
    [TestClass]
    public class StringExtensionTests
    {

        /// <summary>
        /// Defines tests for <see cref="StringExtension.IsURLToImage(string)"/> class.
        /// </summary>
        [TestClass]
        public class IsURLToImageTests
        {
            [TestMethod]
            [DataRow("test", false)]
            [DataRow("https://www.google.com", false)]
            [DataRow("https://www.test.com/image.png", true)]
            public void Should_Check_Url(string url, bool expected)
            {
                url.IsURLToImage().Should().Be(expected);
            }
        }

        /// <summary>
        /// Defines tests for <see cref="StringExtension.IsHexTriplet(string)"/> class.
        /// </summary>
        [TestClass]
        public class IsHexTripletTests
        {
            [TestMethod]
            public void Should_Return_True()
            {
                "#FFFFFF".IsHexTriplet().Should().BeTrue();
            }

            [TestMethod]
            public void Should_Return_False()
            {
                "test".IsHexTriplet().Should().BeFalse();
            }
        }

        /// <summary>
        /// Defines tests for <see cref="StringExtension.ElipseTrimToLength(string, int)"/> class.
        /// </summary>
        [TestClass]
        public class ElipseTrimToLengthTests
        {
            [TestMethod]
            public void Should_Elipse_Text()
            {
                var expected = "aaaaaa...";
                var actual = new string(Enumerable.Repeat('a', 10).ToArray()).ElipseTrimToLength(9);
                actual.Should().Be(expected);
            }

            [TestMethod]
            public void Should_Keep_Text()
            {
                var expected = "aaaaaaaaaa";
                var actual = new string(Enumerable.Repeat('a', 10).ToArray());
                actual.ElipseTrimToLength(10).Should().Be(expected);
            }
        }

        /// <summary>
        /// Defines tests for <see cref="StringExtension.IsCommand(.DiscordConfiguration, string)"/> class.
        /// </summary>
        [TestClass]
        public class IsCommandTests
        {
            private readonly DiscordConfiguration _configuration;

            public IsCommandTests()
            {
                _configuration = new DiscordConfiguration
                {
                    Prefix = ".",
                };
            }

            [TestMethod]
            public void Should_Return_True()
            {
                _configuration.IsCommand(".command").Should().BeTrue();
            }

            [TestMethod]
            public void Should_Return_False()
            {
                _configuration.IsCommand("some text").Should().BeFalse();
            }
        }
    }
}
