using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface IRemoveRepository<T>
    {
        void Remove(T entity);
    }
}
