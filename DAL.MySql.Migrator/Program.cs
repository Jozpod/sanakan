using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Builder;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.MySql.Migrator.Configuration;
using Sanakan.DAL.MySql.Migrator.TableEnumerators;
using Sanakan.DAL.MySql.Migrator.TableMigrators;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Sanakan.DAL.MySql.Migrator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configurationRoot = builder.Build();

            serviceCollection.AddOptions();
            serviceCollection.AddSystemClock();
            serviceCollection.AddSanakanDbContext(configurationRoot);
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.Configure<SourceDatabaseConfiguration>(configurationRoot.GetSection("Source"));
            serviceCollection.AddCache(configurationRoot.GetSection("Cache"));
            serviceCollection.Configure<SanakanConfiguration>(configurationRoot);
            serviceCollection.AddRepositories();
            serviceCollection.AddFileSystem();
            serviceCollection.AddScoped<DatabaseMigrator>();
            serviceCollection.AddScoped<TableEnumerator<Answer>, AnswersEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<Question>, QuestionsEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<Report>, ReportsEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<TimeStatus>, TimeStatusEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<Item>, ItemEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<GameDeck>, GameDecksEnumerators>();
            serviceCollection.AddScoped<TableEnumerator<Figure>, FiguresEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<RarityExcluded>, RarityExcludedEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<User>, UserEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<CardArenaStats>, CardArenaStatsEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<CardPvPStats>, CardPvPStatsEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<CardTag>, CardTagsEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<BoosterPack>, BoosterPackEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<BoosterPackCharacter>, BoosterPackCharacterEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<CommandsAnalytics>, CommandsAnalyticsEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<TransferAnalytics>, TransferAnalyticsEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<SystemAnalytics>, SystemAnalyticsEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<UserAnalytics>, UserAnalyticsEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<SlotMachineConfig>, SlotMachineConfigEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<ExperienceContainer>, ExperienceContainerEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<CommandChannel>, CommandChannelEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<LevelRole>, LevelRolesEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<SelfRole>, SelfRolesEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<OwnedRole>, OwnedRolesEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<PenaltyInfo>, PenaltiesEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<UserStats>, UserStatsEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<WaifuFightChannel>, WaifuFightChannelEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<WishlistObject>, WishlistObjectEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<WithoutExpChannel>, WithoutExpChannelEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<WithoutSupervisionChannel>, WithoutSupervisionChannelEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<WithoutMessageCountChannel>, WithoutMessageCountChannelEnumerator>();
            serviceCollection.AddScoped<TableEnumerator<WaifuCommandChannel>, WaifuCommandChannelEnumerator>();
            serviceCollection.AddScoped((serviceProvider) =>
            {
                var configuration = serviceProvider.GetRequiredService<SourceDatabaseConfiguration>();
                return new MySqlConnection(configuration.ConnectionString);
            });
            serviceCollection.AddScoped((serviceProvider) =>
            {
                var dbContext = serviceProvider.GetRequiredService<SanakanDbContext>();
                return dbContext.Database;
            });

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using var serviceScope = serviceScopeFactory.CreateScope();
            serviceProvider = serviceScope.ServiceProvider;

            var databaseMigrator = serviceProvider.GetRequiredService<DatabaseMigrator>();
            await databaseMigrator.RunAsync();
        }
    }
}
