using Sanakan.DAL.Models.Management;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class OwnedRolesEnumerator : TableEnumerator<OwnedRole>
    {
        public OwnedRolesEnumerator(IDbConnection connection)
         : base(connection)
        {
        }

        public override OwnedRole Current => new()
        {
            RoleId = _reader.GetUInt64(1),
            PenaltyInfoId = _reader.GetUInt64(2),
        };

        public override string TableName => nameof(SanakanDbContext.OwnedRoles);
    }
}
