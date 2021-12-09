using MySqlConnector;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Configuration;
using Sanakan.DAL.MySql.Migrator.TableEnumerators;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class ModeratorRolesEnumerator : TableEnumerator<ModeratorRoles>
    {
        public ModeratorRolesEnumerator(MySqlConnection connection)
         : base(connection) { }

        public override ModeratorRoles Current => new ModeratorRoles
        {
            Id = _reader.GetUInt64(0),
            RoleId = _reader.GetUInt64(1),
            GuildOptionsId = _reader.GetUInt64(2),
        };

        public override string TableName => "commandchannels";
    }
}
