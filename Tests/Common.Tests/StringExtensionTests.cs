using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common;
using Sanakan.Common.Builder;
using System.Threading.Tasks;
using Sanakan.Extensions;

namespace Sanakan.DAL.Tests
{
    /// <summary>
    /// Defines tests for <see cref="StringExtension"/> class.
    /// </summary>
    [TestClass]
    public class StringExtensionTests
    {
        [TestMethod]
        public async Task Should_Return_True()
        {
            "#FFFFFF".IsHexTriplet().Should().BeTrue();
        }

        [TestMethod]
        public async Task False()
        {
            "test".IsHexTriplet().Should().BeFalse();
        }
    }
}
