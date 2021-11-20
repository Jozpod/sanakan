using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Repositories.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class TimeStatusRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_Return_TimeStatus_By_Guild_Id()
        {
            var repository = ServiceProvider.GetRequiredService<ITimeStatusRepository>();

            var actual = await repository.GetByGuildIdAsync(1);
            actual.First().Should().NotBeNull();
        }
    }
}
