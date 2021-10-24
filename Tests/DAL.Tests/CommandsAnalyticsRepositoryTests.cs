using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class CommandsAnalyticsRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_CRUD_Entity()
        {
            var repository = ServiceProvider.GetRequiredService<ICommandsAnalyticsRepository>();
            var entity = new CommandsAnalytics
            {

            };

            repository.Add(entity);
            await repository.SaveChangesAsync();

            var actual = await repository.GetCachedGuildFullConfigAsync(1);
            actual.Should().BeEquivalentTo(entity);
        }
    }
}
