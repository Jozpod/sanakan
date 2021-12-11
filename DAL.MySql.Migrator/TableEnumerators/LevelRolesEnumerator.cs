using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class LevelRolesEnumerator : TableEnumerator<LevelRole>
    {
        public LevelRolesEnumerator(IDbConnection connection)
         : base(connection) { }

        public override LevelRole Current => new()
        {
            RoleId = _reader.GetUInt64(1),
            Level = _reader.GetUInt64(2),
            GuildOptionsId = _reader.GetUInt64(3),
        };

        public override string TableName => nameof(SanakanDbContext.LevelRoles);
    }
}
