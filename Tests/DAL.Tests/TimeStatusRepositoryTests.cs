using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System;
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

            var user = new User(3, DateTime.UtcNow);

            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();

            var entity = new TimeStatus(StatusType.Card);
            entity.GuildId = 1;

            user.TimeStatuses.Add(entity);
            await DbContext.SaveChangesAsync();

            var actual = await repository.GetByGuildIdAsync(1);
            actual.First().Should().BeEquivalentTo(entity);
        }
    }
}
