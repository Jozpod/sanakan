using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql.Schema
{
    public class QueryScripter
    {
        private readonly IFileSystem _fileSystem;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IOptions<DatabaseConfiguration> _databaseConfiguration;

        public QueryScripter(
            IFileSystem fileSystem,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<DatabaseConfiguration> databaseConfiguration)
        {
            _fileSystem = fileSystem;
            _serviceScopeFactory = serviceScopeFactory;
            _databaseConfiguration = databaseConfiguration;
        }

        public async Task RunAsync()
        {
            var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
            var queriesFolder = Path.Combine(path, "Queries");

            using var serviceScope = _serviceScopeFactory.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var connection = new DbConnection(_databaseConfiguration.Value.ConnectionString);
            await connection.OpenAsync();

            await Utils.TruncateGeneralLogAsync(connection);
            await Utils.ToggleGeneralLogAsync(connection, "ON");

            var executed = await ExecuteQueriesAsync(serviceProvider, connection);

            await connection.CloseAsync();
            connection = connection = new DbConnection(_databaseConfiguration.Value.ConnectionString);
            await connection.OpenAsync();
            await Utils.ToggleGeneralLogAsync(connection, "OFF");

            var queries = await Utils.GetLastQueriesAsync(connection);

            foreach (var (queryName, query) in Enumerable.Zip(executed, queries))
            {
                var filePath = Path.Combine(queriesFolder, $"{queryName}.sql");
                await _fileSystem.WriteAllTextAsync(filePath, query);
            }

            await connection.CloseAsync();
        }

        private async Task<IEnumerable<string>> ExecuteQueriesAsync(IServiceProvider serviceProvider, DbConnection connection)
        {
            var executed = new List<string>();
            var userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            var cardRepository = serviceProvider.GetRequiredService<ICardRepository>();
            var guildConfigRepository = serviceProvider.GetRequiredService<IGuildConfigRepository>();
            var penaltyInfoRepository = serviceProvider.GetRequiredService<IPenaltyInfoRepository>();
            var timeStatusRepository = serviceProvider.GetRequiredService<ITimeStatusRepository>();
            var gameDeckRepository = serviceProvider.GetRequiredService<IGameDeckRepository>();

            await userRepository.GetByDiscordIdAsync(1ul);
            await Utils.StubSelectAsync(connection);
            executed.Add($"{nameof(IUserRepository)}_{nameof(IUserRepository.GetByDiscordIdAsync)}");

            await userRepository.GetByShindenIdAsync(1ul);
            await Utils.StubSelectAsync(connection);
            executed.Add($"{nameof(IUserRepository)}_{nameof(IUserRepository.GetByShindenIdAsync)}");

            await cardRepository.GetCardsByCharacterIdAsync(1ul);
            await Utils.StubSelectAsync(connection);
            executed.Add($"{nameof(ICardRepository)}_{nameof(ICardRepository.GetCardsByCharacterIdAsync)}");

            await cardRepository.GetByCharactersAndNotInUserGameDeckAsync(1ul, new[] { 1ul });
            await Utils.StubSelectAsync(connection);
            executed.Add($"{nameof(ICardRepository)}_{nameof(ICardRepository.GetByCharactersAndNotInUserGameDeckAsync)}");

            await gameDeckRepository.GetByAnimeIdAsync(1ul);
            await Utils.StubSelectAsync(connection);
            executed.Add($"{nameof(IGameDeckRepository)}_{nameof(IGameDeckRepository.GetByAnimeIdAsync)}");

            await gameDeckRepository.GetByCardIdAndCharacterAsync(1ul, 1ul);
            await Utils.StubSelectAsync(connection);
            executed.Add($"{nameof(IGameDeckRepository)}_{nameof(IGameDeckRepository.GetByCardIdAndCharacterAsync)}");

            await gameDeckRepository.GetCachedPlayersForPVP();
            await Utils.StubSelectAsync(connection);
            executed.Add($"{nameof(IGameDeckRepository)}_{nameof(IGameDeckRepository.GetCachedPlayersForPVP)}");

            await guildConfigRepository.GetCachedGuildFullConfigAsync(1ul);
            await Utils.StubSelectAsync(connection);
            executed.Add($"{nameof(IGuildConfigRepository)}_{nameof(IGuildConfigRepository.GetCachedGuildFullConfigAsync)}");

            await penaltyInfoRepository.GetByGuildIdAsync(1ul);
            await Utils.StubSelectAsync(connection);
            executed.Add($"{nameof(IPenaltyInfoRepository)}_{nameof(IPenaltyInfoRepository.GetByGuildIdAsync)}");

            await penaltyInfoRepository.GetMutedPenaltiesAsync(1ul);
            await Utils.StubSelectAsync(connection);
            executed.Add($"{nameof(IPenaltyInfoRepository)}_{nameof(IPenaltyInfoRepository.GetMutedPenaltiesAsync)}");

            await timeStatusRepository.GetByGuildIdAsync(1ul);
            await Utils.StubSelectAsync(connection);
            executed.Add($"{nameof(ITimeStatusRepository)}_{nameof(ITimeStatusRepository.GetByGuildIdAsync)}");

            await timeStatusRepository.GetBySubTypeAsync();
            await Utils.StubSelectAsync(connection);
            executed.Add($"{nameof(ITimeStatusRepository)}_{nameof(ITimeStatusRepository.GetBySubTypeAsync)}");

            return executed;
        }
    }
}
