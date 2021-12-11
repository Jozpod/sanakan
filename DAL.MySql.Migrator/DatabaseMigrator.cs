using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.MySql.Migrator.TableEnumerators;
using System;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Migrator
{
    public class DatabaseMigrator : IAsyncDisposable
    {
        private readonly ILogger<DatabaseMigrator> _logger;
        private readonly SanakanDbContext _dbContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly int _batchCount;

        public DatabaseMigrator(
            ILogger<DatabaseMigrator> logger,
            SanakanDbContext dbContext,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _dbContext = dbContext;
            _serviceScopeFactory = serviceScopeFactory;
            _batchCount = 100;
        }

        public async Task RunAsync()
        {
            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            await MigrateTableAsync<User>(serviceProvider);
            await MigrateTableAsync<GuildOptions>(serviceProvider);
            await MigrateTableAsync<GameDeck>(serviceProvider);
            await MigrateTableAsync<Card>(serviceProvider);
            await MigrateTableAsync<CardArenaStats>(serviceProvider);
            await MigrateTableAsync<CardPvPStats>(serviceProvider);
            await MigrateTableAsync<CardTag>(serviceProvider);
            await MigrateTableAsync<WaifuConfiguration>(serviceProvider);
            await MigrateTableAsync<WaifuCommandChannel>(serviceProvider);
            await MigrateTableAsync<WithoutExpChannel>(serviceProvider);
            await MigrateTableAsync<WithoutSupervisionChannel>(serviceProvider);
            await MigrateTableAsync<WaifuFightChannel>(serviceProvider);
            await MigrateTableAsync<BoosterPack>(serviceProvider);
            await MigrateTableAsync<BoosterPackCharacter>(serviceProvider);
            await MigrateTableAsync<CommandChannel>(serviceProvider);
            await MigrateTableAsync<ExperienceContainer>(serviceProvider);
            await MigrateTableAsync<Figure>(serviceProvider);
            await MigrateTableAsync<WithoutMessageCountChannel>(serviceProvider);
            await MigrateTableAsync<Item>(serviceProvider);
            await MigrateTableAsync<UserLand>(serviceProvider);
            await MigrateTableAsync<PenaltyInfo>(serviceProvider);
            await MigrateTableAsync<Question>(serviceProvider);
            await MigrateTableAsync<Answer>(serviceProvider);
            await MigrateTableAsync<Report>(serviceProvider);
            await MigrateTableAsync<RarityExcluded>(serviceProvider);
            await MigrateTableAsync<SelfRole>(serviceProvider);
            await MigrateTableAsync<LevelRole>(serviceProvider);
            await MigrateTableAsync<ModeratorRoles>(serviceProvider);
            await MigrateTableAsync<OwnedRole>(serviceProvider);
            await MigrateTableAsync<SlotMachineConfig>(serviceProvider);
            await MigrateTableAsync<SystemAnalytics>(serviceProvider);
            await MigrateTableAsync<CommandsAnalytics>(serviceProvider);
            await MigrateTableAsync<TransferAnalytics>(serviceProvider);
            await MigrateTableAsync<UserAnalytics>(serviceProvider);
            await MigrateTableAsync<TimeStatus>(serviceProvider);
            await MigrateTableAsync<UserStats>(serviceProvider);
            await MigrateTableAsync<WishlistObject>(serviceProvider);
        }

        public async Task MigrateTableAsync<T>(IServiceProvider serviceProvider)
            where T : class
        {
            var dbSet = _dbContext.Set<T>();
            await using var tableEnumerator = serviceProvider.GetService<TableEnumerator<T>>();
            await tableEnumerator.OpenAsync();
            var currentIt = 0;

            await foreach (var record in tableEnumerator)
            {
                dbSet.Add(record);
                _logger.LogInformation("Adding {0} entity", record);

                if (currentIt % _batchCount == 0)
                {
                    _logger.LogInformation("Saving batch");
                    await _dbContext.SaveChangesAsync();
                }

                currentIt++;
            }

            if (currentIt % _batchCount != 0)
            {
                _logger.LogInformation("Saving batch");
                await _dbContext.SaveChangesAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }
    }
}
