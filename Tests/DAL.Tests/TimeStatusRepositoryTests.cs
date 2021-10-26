using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class TimeStatusRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_CRUD_Entity()
        {
            var repository = ServiceProvider.GetRequiredService<ITimeStatusRepository>();
            var entity = new TimeStatus
            {
                Id = 1,
                UserId = 1,
                BValue = true,
            };

            DbContext.TimeStatuses.Add(entity);
            await DbContext.SaveChangesAsync();

            var actual = await repository.GetByGuildIdAsync(1);
            actual.First().Should().BeEquivalentTo(entity);
        }
    }
}
