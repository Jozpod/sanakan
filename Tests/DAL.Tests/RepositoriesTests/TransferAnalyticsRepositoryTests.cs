﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class TransferAnalyticsRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_Add_Entity()
        {
            var repository = serviceProvider.GetRequiredService<ITransferAnalyticsRepository>();
            var entity = new TransferAnalytics
            {
                Id = 1,
                CreatedOn = DateTime.UtcNow,
                DiscordUserId = 1,
                ShindenUserId = 1,
                Source = TransferSource.ByDiscordId,
                Value = 10,
            };

            repository.Add(entity);
            await repository.SaveChangesAsync();
        }
    }
}
