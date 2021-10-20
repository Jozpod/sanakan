using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.DAL.Repositories
{
    public class CommandsAnalyticsRepository : 
        BaseRepository<CommandsAnalytics>, ICommandsAnalyticsRepository
    {
        private readonly BuildDatabaseContext _dbContext;

        public CommandsAnalyticsRepository(
            BuildDatabaseContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
