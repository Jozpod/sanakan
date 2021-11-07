using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class UserAnalyticsRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_Add_Entity()
        {
            var repository = ServiceProvider.GetRequiredService<IUserAnalyticsRepository>();
            var entity = new UserAnalytics
            {
                Id = 1,
                GuildId = 1,
                MeasureDate = DateTime.UtcNow,
                Type = UserAnalyticsEventType.Card,
                UserId = 1,
                Value = 10,
            };

            repository.Add(entity);
            await repository.SaveChangesAsync();
        }
    }
}
