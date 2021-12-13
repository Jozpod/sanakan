using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using Sanakan.DAL;
using Sanakan.DAL.Builder;
using Sanakan.DAL.MySql;
using Sanakan.DAL.MySql.Builder;
using Sanakan.DAL.MySql.Migrator;
using Sanakan.DAL.MySql.Migrator.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DAL.MySql.Migrator.Tests
{
#if DEBUG
    [TestClass]
#endif
    public class DatabaseMigratorTests
    {
        private IServiceProvider _serviceProvider = null;
        private DatabaseMigrator _databaseMigrator = null;
        private SanakanDbContext _dbContext = null;
        private TestDatabaseConfiguration _testDatabaseConfiguration = null;
        private Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade _databaseFacade = null;

        [TestInitialize]
        public async Task TestInitialize()
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<TestDatabaseConfiguration>();

            var configurationRoot = builder.Build();

            serviceCollection.AddLogging();
            serviceCollection.AddOptions();
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.AddScoped<DatabaseMigrator>();
            serviceCollection.Configure<TestDatabaseConfiguration>(configurationRoot.GetSection(nameof(TestDatabaseConfiguration)));
            serviceCollection.Configure<SourceDatabaseConfiguration>(configurationRoot.GetSection("SourceDatabase"));
            serviceCollection.AddSanakanDbContext(configurationRoot);
            serviceCollection.AddDatabaseMigrator();

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _databaseMigrator = _serviceProvider.GetRequiredService<DatabaseMigrator>();
            _dbContext = _serviceProvider.GetRequiredService<SanakanDbContext>();
            _testDatabaseConfiguration = _serviceProvider.GetRequiredService<IOptions<TestDatabaseConfiguration>>().Value;
            _databaseFacade = _dbContext.Database;
            await GenerateSourceAndTargetDatabaseAsync();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            await RemoveDatabasesAsync();
        }

        public async Task RemoveDatabasesAsync()
        {
            var connection = new DbConnection(_testDatabaseConfiguration.ServerConnectionString);

            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = $"DROP DATABASE `{_testDatabaseConfiguration.DatabaseName}`";
            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();

            await _databaseFacade.EnsureDeletedAsync();
            await _dbContext.DisposeAsync();
        }

        public async Task GenerateSourceAndTargetDatabaseAsync()
        {
            var created = await _databaseFacade.EnsureCreatedAsync();

            if (!created)
            {
                return;
            }

            var connection = new DbConnection(_testDatabaseConfiguration.ServerConnectionString);

            await connection.OpenAsync();
            var scriptText = await File.ReadAllTextAsync("CreateDatabase.sql");
            var script = new MySqlScript(connection.GetBaseConnection(), scriptText);
            await script.ExecuteAsync();
            await connection.CloseAsync();

            connection = new DbConnection(_testDatabaseConfiguration.SourceDatabaseConnectionString);
            await connection.OpenAsync();
            scriptText = await File.ReadAllTextAsync("GenerateSampleData.sql");
            script = new MySqlScript(connection.GetBaseConnection(), scriptText);
            await script.ExecuteAsync();
            await connection.CloseAsync();
        }

        [TestMethod]
        public async Task Should_Migrate_Database()
        {
            await _databaseMigrator.RunAsync();

            (await _dbContext.Users.AnyAsync()).Should().BeTrue();
            (await _dbContext.UsersData.AnyAsync()).Should().BeTrue();
            (await _dbContext.UsersStats.AnyAsync()).Should().BeTrue();
            (await _dbContext.Figures.AnyAsync()).Should().BeTrue();
            (await _dbContext.RaritysExcludedFromPacks.AnyAsync()).Should().BeTrue();
            (await _dbContext.Raports.AnyAsync()).Should().BeTrue();
            (await _dbContext.BoosterPacks.AnyAsync()).Should().BeTrue();
            (await _dbContext.Questions.AnyAsync()).Should().BeTrue();
            (await _dbContext.Items.AnyAsync()).Should().BeTrue();
            (await _dbContext.Answers.AnyAsync()).Should().BeTrue();
            (await _dbContext.CardArenaStats.AnyAsync()).Should().BeTrue();
            (await _dbContext.CardPvPStats.AnyAsync()).Should().BeTrue();
            (await _dbContext.Cards.AnyAsync()).Should().BeTrue();
            (await _dbContext.CardTags.AnyAsync()).Should().BeTrue();
            (await _dbContext.CommandChannels.AnyAsync()).Should().BeTrue();
            (await _dbContext.CommandsData.AnyAsync()).Should().BeTrue();
            (await _dbContext.BoosterPackCharacters.AnyAsync()).Should().BeTrue();
            (await _dbContext.ExperienceContainers.AnyAsync()).Should().BeTrue();
            (await _dbContext.SlotMachineConfigs.AnyAsync()).Should().BeTrue();
            (await _dbContext.IgnoredChannels.AnyAsync()).Should().BeTrue();
            (await _dbContext.WaifuCommandChannels.AnyAsync()).Should().BeTrue();
            (await _dbContext.WaifuFightChannels.AnyAsync()).Should().BeTrue();
            (await _dbContext.Waifus.AnyAsync()).Should().BeTrue();
            (await _dbContext.Wishes.AnyAsync()).Should().BeTrue();
            (await _dbContext.WithoutExpChannels.AnyAsync()).Should().BeTrue();
            (await _dbContext.WithoutSupervisionChannels.AnyAsync()).Should().BeTrue();
            (await _dbContext.GameDecks.AnyAsync()).Should().BeTrue();
            (await _dbContext.Guilds.AnyAsync()).Should().BeTrue();
            (await _dbContext.TimeStatuses.AnyAsync()).Should().BeTrue();
            (await _dbContext.TransferData.AnyAsync()).Should().BeTrue();
            (await _dbContext.Penalties.AnyAsync()).Should().BeTrue();
            (await _dbContext.LevelRoles.AnyAsync()).Should().BeTrue();
            (await _dbContext.ModeratorRoles.AnyAsync()).Should().BeTrue();
            (await _dbContext.OwnedRoles.AnyAsync()).Should().BeTrue();
            (await _dbContext.SelfRoles.AnyAsync()).Should().BeTrue();
            (await _dbContext.SystemData.AnyAsync()).Should().BeTrue();

        }
    }
}
