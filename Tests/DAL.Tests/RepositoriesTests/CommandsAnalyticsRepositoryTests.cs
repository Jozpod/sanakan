using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class CommandsAnalyticsRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_Add_Entity()
        {
            var repository = ServiceProvider.GetRequiredService<ICommandsAnalyticsRepository>();
            var entity = new CommandsAnalytics
            {
                Id = 1,
                CommandName = "test",
                Date = DateTime.UtcNow,
                GuildId = 1,
                UserId = 1,
            };

            repository.Add(entity);
            await repository.SaveChangesAsync();
        }
    }
}
