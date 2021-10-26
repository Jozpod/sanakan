using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class PenaltyInfoRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_CRUD_Entity()
        {
            var repository = ServiceProvider.GetRequiredService<IPenaltyInfoRepository>();
            var entity = new PenaltyInfo
            {
                Id = 1,
                Duration = TimeSpan.FromDays(1),
                GuildId = 2,
                Reason = "test",
                StartDate = DateTime.UtcNow,
                UserId = 1,
            };

            repository.Add(entity);
            await repository.SaveChangesAsync();
            var actual = await repository.GetByGuildIdAsync(2);
            actual.First().Should().BeEquivalentTo(entity);
        }
    }
}
