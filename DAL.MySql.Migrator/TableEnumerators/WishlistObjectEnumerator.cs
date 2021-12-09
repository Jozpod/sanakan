using MySqlConnector;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class WishlistObjectEnumerator : TableEnumerator<WishlistObject>
    {
        public WishlistObjectEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override WishlistObject Current => new WishlistObject
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.Wishes);
    }
}
