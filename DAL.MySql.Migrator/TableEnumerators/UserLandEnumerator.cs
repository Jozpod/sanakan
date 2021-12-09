using MySqlConnector;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class UserLandEnumerator : TableEnumerator<UserLand>
    {
        public UserLandEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override UserLand Current => new UserLand
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.UserLands);
    }
}
