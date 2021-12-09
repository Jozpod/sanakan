using MySqlConnector;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class CardEnumerator : TableEnumerator<Card>
    {
        public CardEnumerator(MySqlConnection connection)
           : base(connection) { }

        public override Card Current => new Card
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.Cards);
    }
}
