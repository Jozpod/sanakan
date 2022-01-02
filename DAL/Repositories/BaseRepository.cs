using Microsoft.EntityFrameworkCore;
using Sanakan.DAL.Repositories.Abstractions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories
{
    public class BaseRepository<T> :
        ICreateRepository<T>, IRemoveRepository<T>, ISaveRepository
        where T : class
    {
        private readonly SanakanDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public BaseRepository(SanakanDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public void Add(T entity) => _dbSet.Add(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);

        public void RemoveRange(IEnumerable<T> entity) => _dbSet.RemoveRange(entity);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _dbContext.SaveChangesAsync(cancellationToken);
    }
}
