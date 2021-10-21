using System.Collections.Generic;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IRemoveRepository<T>
    {
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
    }
}
