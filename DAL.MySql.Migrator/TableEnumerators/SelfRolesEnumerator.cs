using MySqlConnector;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class SelfRolesEnumerator : TableEnumerator<SelfRole>
    {
        public SelfRolesEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override SelfRole Current => new SelfRole
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.SelfRoles);
    }
}
