using System;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql
{
    public interface IDbCommand : IDisposable, IAsyncDisposable
    {
        Task<int> ExecuteNonQueryAsync();
        
        Task<IDbDataReader> ExecuteReaderAsync();

        string CommandText { get; set; }
    }
}
