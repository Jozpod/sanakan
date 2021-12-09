using MySqlConnector;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class UserStatsEnumerator : TableEnumerator<UserStats>
    {
        public UserStatsEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override UserStats Current => new UserStats
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.UsersData);
    }
}
