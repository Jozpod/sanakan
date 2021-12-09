using MySqlConnector;
using Sanakan.DAL.Models.Analytics;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class UserAnalyticsEnumerator : TableEnumerator<UserAnalytics>
    {
        public UserAnalyticsEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override UserAnalytics Current => new UserAnalytics
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.UsersData);
    }
}
