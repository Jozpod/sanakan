using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class CardTagsEnumerator : TableEnumerator<CardTag>
    {
        public CardTagsEnumerator(IDbConnection connection)
          : base(connection) { }

        public override CardTag Current => new()
        {
            Name = _reader.GetString(1),
            CardId = _reader.GetUInt64(2),
        };

        public override string TableName => nameof(SanakanDbContext.CardTags);
    }
}
