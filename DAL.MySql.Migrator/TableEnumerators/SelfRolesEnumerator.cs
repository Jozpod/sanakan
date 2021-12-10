using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class SelfRolesEnumerator : TableEnumerator<SelfRole>
    {
        public SelfRolesEnumerator(IDbConnection connection)
            : base(connection) { }

        public override SelfRole Current => new()
        {
            Id = _reader.GetUInt64(0),
            RoleId = _reader.GetUInt64(1),
            Name = _reader.GetString(2),
            GuildOptionsId = _reader.GetUInt64(3),
        };

        public override string TableName => nameof(SanakanDbContext.SelfRoles);
    }
}
