using MySqlConnector;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class CardTagsEnumerator : TableEnumerator<CardTag>
    {
        public CardTagsEnumerator(MySqlConnection connection)
          : base(connection) { }

        public override CardTag Current => new CardTag
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.CardTags);
    }
}
