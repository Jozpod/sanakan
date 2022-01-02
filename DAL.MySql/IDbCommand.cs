using System;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql
{
    public interface IDbCommand : IDisposable, IAsyncDisposable
    {
        string CommandText { get; set; }

        Task<int> ExecuteNonQueryAsync();

        Task<IDbDataReader> ExecuteReaderAsync();
    }
}
