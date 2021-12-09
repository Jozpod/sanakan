using MySqlConnector;
using Sanakan.DAL.Models.Analytics;
using Sanakan.DAL.MySql.Migrator.TableEnumerators;

namespace Sanakan.DAL.MySql.Migrator.TableMigrators
{
    public class TransferAnalyticsEnumerator : TableEnumerator<TransferAnalytics>
    {
        public TransferAnalyticsEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override TransferAnalytics Current => new TransferAnalytics
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.TransferData);
    }
}
