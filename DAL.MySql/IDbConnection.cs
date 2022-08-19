using System;
using System.Data;
using System.Threading.Tasks;

namespace Sanakan.DAL.MySql
{
    public interface IDbConnection : IDisposable, IAsyncDisposable
    {
        public ConnectionState State { get; }

        IDbCommand CreateCommand();

        public Task OpenAsync();

        public Task CloseAsync();
    }
}
