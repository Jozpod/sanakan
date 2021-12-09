using MySqlConnector;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class CardPvPStatsEnumerator : TableEnumerator<CardPvPStats>
    {
        public CardPvPStatsEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override CardPvPStats Current => new CardPvPStats
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.CardPvPStats);
    }
}
