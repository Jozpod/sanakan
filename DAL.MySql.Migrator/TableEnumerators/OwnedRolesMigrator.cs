using MySqlConnector;
using Sanakan.DAL.Models.Management;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class OwnedRolesEnumerator : TableEnumerator<OwnedRole>
    {
        public OwnedRolesEnumerator(MySqlConnection connection)
         : base(connection) { }

        public override OwnedRole Current => new OwnedRole
        {
            Id = _reader.GetUInt64(0),
            RoleId = _reader.GetUInt64(1),
            PenaltyInfoId = _reader.GetUInt64(2),
        };

        public override string TableName => "commandchannels";
    }
}
