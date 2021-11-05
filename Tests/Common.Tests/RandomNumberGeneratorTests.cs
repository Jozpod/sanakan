using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Common;
using Sanakan.Common.Builder;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class RandomNumberGeneratorTests
    {
        private ServiceProvider _serviceProvider;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public RandomNumberGeneratorTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddRandomNumberGenerator();
            _serviceProvider = serviceCollection.BuildServiceProvider();
            _randomNumberGenerator = _serviceProvider.GetRequiredService<IRandomNumberGenerator>();
        }

        [TestMethod]
        public void Should_Get_Random_Value()
        {
            var result = _randomNumberGenerator.GetRandomValue(100);
            result = result == 0 ? _randomNumberGenerator.GetRandomValue(100) : result;
            result.Should().NotBe(0);
        }

        [TestMethod]
        public void Should_Get_Random_Element()
        {
            var items = new[] { 1, 2, 3 };
            var result = _randomNumberGenerator.GetOneRandomFrom(items);
            result.Should().NotBe(0);
        }
    }
}
