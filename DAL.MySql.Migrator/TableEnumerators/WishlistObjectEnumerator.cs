using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class WishlistObjectEnumerator : TableEnumerator<WishlistObject>
    {
        public WishlistObjectEnumerator(IDbConnection connection)
            : base(connection) { }

        public override WishlistObject Current => new WishlistObject
        {
            Id = _reader.GetUInt64(0),
            ObjectId = _reader.GetUInt64(1),
            ObjectName = _reader.GetString(2),
            Type = (WishlistObjectType)_reader.GetInt32(3),
            GameDeckId = _reader.GetUInt64(4),
        };

        public override string TableName => nameof(SanakanDbContext.Wishes);
    }
}
