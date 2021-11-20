using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Repositories.Abstractions;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class GuildConfigRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_Return_Cached_Guild_Config_By_Id()
        {
            var repository = ServiceProvider.GetRequiredService<IGuildConfigRepository>();
            var actual = await repository.GetCachedGuildFullConfigAsync(1);
            actual.Should().NotBeNull();
        }
    }
}
