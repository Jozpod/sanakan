using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class GuildConfigRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_CRUD_Entity()
        {
            var repository = ServiceProvider.GetRequiredService<IGuildConfigRepository>();
            var entity = new GuildOptions(1, 50);

            repository.Add(entity);
            await repository.SaveChangesAsync();

            var actual = await repository.GetCachedGuildFullConfigAsync(1);
            actual.Should().BeEquivalentTo(entity);
        }
    }
}
