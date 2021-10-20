using Microsoft.EntityFrameworkCore;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    public class BaseRepository<T> : 
        ICreateRepository<T>, IRemoveRepository<T>, ISaveRepository
        where T : class
    {
        private readonly BuildDatabaseContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(BuildDatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public void Add(T entity) => _dbSet.Add(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);

        public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
    }
}
