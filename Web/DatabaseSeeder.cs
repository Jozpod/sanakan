using Microsoft.Extensions.Options;
using Sanakan.Common.Configuration;
using Sanakan.DAL;
using Sanakan.DAL.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web
{
    [ExcludeFromCodeCoverage]
    public class DatabaseSeeder
    {
        private readonly SanakanDbContext _dbContext;
        private readonly IDatabaseFacade _databaseFacade;
        private readonly DatabaseSeedConfiguration _configuration;

        public DatabaseSeeder(
            SanakanDbContext dbContext,
            IDatabaseFacade databaseFacade,
            IOptions<DatabaseSeedConfiguration> configuration)
        {
            _dbContext = dbContext;
            _databaseFacade = databaseFacade;
            _configuration = configuration.Value;
        }

        public async Task RunAsync()
        {
            if (_configuration.Enabled)
            {
                return;
            }

            if(!await _databaseFacade.EnsureCreatedAsync())
            {
                return;
            }

            foreach (var guild in _configuration.Guilds)
            {
                var guildOption = new GuildOptions(guild.Id, guild.SafariLimit);

                _dbContext.Guilds.Add(guildOption);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
