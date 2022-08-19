using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class UserLandEnumerator : TableEnumerator<UserLand>
    {
        public UserLandEnumerator(IDbConnection connection)
            : base(connection)
        {
        }

        public override UserLand Current => new()
        {
            Id = _reader.GetUInt64(0),
            Name = _reader.GetString(1),
            ManagerId = _reader.GetUInt64(2),
            UnderlingId = _reader.GetUInt64(3),
            GuildOptionsId = _reader.GetUInt64(4),
        };

        public override string TableName => "mylands";
    }
}
