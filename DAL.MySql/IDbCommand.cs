using System.Threading.Tasks;

namespace Sanakan.DAL.MySql
{
    public interface IDbCommand
    {
        Task<int> ExecuteNonQueryAsync();
        
        Task<IDbDataReader> ExecuteReaderAsync();

        string CommandText { get; set; }
    }
}
