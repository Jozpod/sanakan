using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface ICreateRepository<T>
    {
        void Add(T entity);
        Task SaveChangesAsync();
    }
}
