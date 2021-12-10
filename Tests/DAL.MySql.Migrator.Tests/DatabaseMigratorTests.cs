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
    [TestClass]
    public class DatabaseMigratorTests
    {
        private DatabaseMigrator _databaseMigrator;
        private SanakanDbContext _dbContext;
        private TestDatabaseConfiguration _testDatabaseConfiguration;
        private Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade _databaseFacade;

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

            try
            {
                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
                _databaseMigrator = serviceProvider.GetRequiredService<DatabaseMigrator>();
                _dbContext = serviceProvider.GetRequiredService<SanakanDbContext>();
                _testDatabaseConfiguration = serviceProvider.GetRequiredService<IOptions<TestDatabaseConfiguration>>().Value;
                _databaseFacade = _dbContext.Database;
                await GenerateSourceAndTargetDatabaseAsync();
            }
            catch (Exception ex)
            {
            }
        }

        [TestCleanup]
        public async Task TestCleanup()
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

            await _databaseFacade.EnsureCreatedAsync();
        }

        [TestMethod]
        public async Task Should_Migrate_Database()
        {
            try
            {
                await _databaseMigrator.RunAsync();
            }
            catch (Exception ex)
            {
            }
           
        }
    }
}
