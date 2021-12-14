using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Extensions;

namespace Sanakan.Common.Tests
{
    /// <summary>
    /// Defines tests for <see cref="StringExtension"/> class.
    /// </summary>
    [TestClass]
    public class StringExtensionTests
    {
        /// <summary>
        /// Defines tests for <see cref="StringExtension.IsHexTriplet(string)"/> class.
        /// </summary>
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
    }
}
