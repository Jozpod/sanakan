using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class ModeratorRolesEnumerator : TableEnumerator<ModeratorRoles>
    {
        public ModeratorRolesEnumerator(IDbConnection connection)
         : base(connection)
        {
        }

        public override ModeratorRoles Current => new()
        {
            RoleId = _reader.GetUInt64(1),
            GuildOptionsId = _reader.GetUInt64(2),
        };

        public override string TableName => nameof(SanakanDbContext.ModeratorRoles);
    }
}
