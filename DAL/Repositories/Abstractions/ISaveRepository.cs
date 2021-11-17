using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface ISaveRepository
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
