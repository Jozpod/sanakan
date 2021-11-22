using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sanakan.Common;
using Sanakan.Common.Builder;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Builder;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.Models.Management;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sanakan.DAL.MySql.Schema
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
            serviceCollection.AddCache(configurationRoot.GetSection("Cache"));
            serviceCollection.Configure<SanakanConfiguration>(configurationRoot);
            serviceCollection.AddRepositories();
            serviceCollection.AddFileSystem();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
            var dbContext = serviceProvider.GetRequiredService<SanakanDbContext>();
            var databaseFacade = dbContext.Database;
            await databaseFacade.EnsureCreatedAsync();
            var connection = databaseFacade.GetDbConnection();
            await connection.OpenAsync();

            var tableNames = await Utils.GetTableNamesAsync(connection);

            var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            var tablesFolder = Path.Combine(path, "Tables");

            if (!fileSystem.DirectoryExists(tablesFolder))
            {
                fileSystem.CreateDirectory(tablesFolder);
            }

            var stringBuilder = new StringBuilder(1000);

            //foreach (var tableName in tableNames)
            //{
            //    var tableDefinition = await Utils.GetTableDefinitionAsync(connection, tableName);
            //    var filePath = Path.Combine(tablesFolder, $"{tableName}.sql");

            //    var tableIndexes = await Utils.GetTableIndexesAsync(connection, tableName);

            //    stringBuilder.AppendLine(tableDefinition);

            //    foreach (var tableIndex in tableIndexes)
            //    {
            //        stringBuilder.AppendLine(tableIndex);
            //    }

            //    await fileSystem.WriteAllTextAsync(filePath, stringBuilder.ToString());
            //    stringBuilder.Clear();
            //}

            await PopulateTestDataAsync(dbContext);

            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            var cardRepository = serviceProvider.GetRequiredService<ICardRepository>();
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            var penaltyInfoRepository = serviceProvider.GetRequiredService<IPenaltyInfoRepository>();

            await Utils.TruncateGeneralLogAsync(connection);
            await Utils.ToggleGeneralLogAsync(connection, "ON");

            var discordUser = await userRepository.GetByDiscordIdAsync(1ul);
            await Utils.StubSelectAsync(connection);
            var shindenUser = await userRepository.GetByShindenIdAsync(1ul);
            await Utils.StubSelectAsync(connection);
            var card = await cardRepository.GetCardsByCharacterIdAsync(1ul);
            await Utils.StubSelectAsync(connection);
            var guildConfig = await guildConfigRepository.GetCachedGuildFullConfigAsync(1ul);
            await Utils.StubSelectAsync(connection);
            var penalties = await penaltyInfoRepository.GetByGuildIdAsync(1ul);
            await Utils.StubSelectAsync(connection);

            var repositoryMethods = new[]
            {
                $"{nameof(IUserRepository)}_{nameof(IUserRepository.GetByDiscordIdAsync)}",
                $"{nameof(IUserRepository)}_{nameof(IUserRepository.GetByShindenIdAsync)}",
                $"{nameof(ICardRepository)}_{nameof(ICardRepository.GetCardsByCharacterIdAsync)}",
                $"{nameof(IGuildConfigRepository)}_{nameof(IGuildConfigRepository.GetCachedGuildFullConfigAsync)}",
                $"{nameof(IPenaltyInfoRepository)}_{nameof(IPenaltyInfoRepository.GetByGuildIdAsync)}",
            };

            await connection.CloseAsync();
            connection = databaseFacade.GetDbConnection();
            await connection.OpenAsync();
            await Utils.ToggleGeneralLogAsync(connection, "OFF");

            var queriesFolder = Path.Combine(path, "Queries");
            var queries = await Utils.GetLastQueriesAsync(connection);

            foreach (var (repositoryMethod, query) in Enumerable.Zip(repositoryMethods, queries))
            {
                var filePath = Path.Combine(queriesFolder, $"{repositoryMethod}.sql");
                await fileSystem.WriteAllTextAsync(filePath, query);
            }

            await connection.CloseAsync();
            await databaseFacade.EnsureDeletedAsync();
        }

        public static async Task PopulateTestDataAsync(SanakanDbContext dbContext)
        {
            var user1 = new User(1, DateTime.UtcNow);
            var user2 = new User(2, DateTime.UtcNow);
            user2.ShindenId = 1ul;
            var guildConfig = new GuildOptions(1, 50);
            var card = new Card(
            1, "test card", "test card",
            10, 20, Rarity.A,
            Dere.Bodere, DateTime.UtcNow);
            var penalty = new PenaltyInfo
            {
                UserId = 1ul,
                Reason = "test",
                GuildId = 1ul,
                Type = PenaltyType.Ban,
                StartedOn = DateTime.UtcNow,
                Duration = TimeSpan.FromMinutes(10),
            };

            dbContext.Users.Add(user1);
            dbContext.Users.Add(user2);
            await dbContext.SaveChangesAsync();

            dbContext.Guilds.Add(guildConfig);
            await dbContext.SaveChangesAsync();

            card.GameDeckId = user1.GameDeck.Id;
            dbContext.Cards.Add(card);
            await dbContext.SaveChangesAsync();

            dbContext.Penalties.Add(penalty);
            await dbContext.SaveChangesAsync();
        }
    }
}
