using MySqlConnector;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class ItemEnumerator : TableEnumerator<Item>
    {
        public ItemEnumerator(MySqlConnection connection)
             : base(connection) { }

        public override Item Current => new Item
        {
            Id = _reader.GetUInt64(0),
            Count = _reader.GetInt64(1),
            Name = _reader.GetString(2),
            Type = (ItemType)_reader.GetInt32(3),
            Quality = (Quality)_reader.GetInt32(4),
            GameDeckId = _reader.GetUInt64(5),
        };

        public override string TableName => "Items";
    }
}
