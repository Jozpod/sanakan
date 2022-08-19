using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class SystemAnalyticsRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_Add_Entity()
        {
            var repository = serviceProvider.GetRequiredService<ISystemAnalyticsRepository>();
            var entity = new SystemAnalytics
            {
                Id = 1,
                MeasuredOn = DateTime.UtcNow,
                Type = SystemAnalyticsEventType.Ram,
                Value = 1000,
            };

            repository.Add(entity);
            await repository.SaveChangesAsync();
        }
    }
}
