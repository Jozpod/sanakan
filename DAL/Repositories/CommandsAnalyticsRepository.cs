using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.DAL.Repositories
{
    public class CommandsAnalyticsRepository : ICommandsAnalyticsRepository
    {
        private readonly BuildDatabaseContext _dbContext;

        public CommandsAnalyticsRepository(
            BuildDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
