using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.DAL.Repositories
{
    public class UserAnalyticsRepository : IUserAnalyticsRepository
    {
        private readonly BuildDatabaseContext _dbContext;

        public UserAnalyticsRepository(
            BuildDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
