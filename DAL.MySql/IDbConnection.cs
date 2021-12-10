
using System;
using System.Data;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql
{
    public interface IDbConnection : IDisposable, IAsyncDisposable
    {
        IDbCommand CreateCommand();
        public Task OpenAsync();
        public Task CloseAsync();
        public ConnectionState State { get; }
    }
}
