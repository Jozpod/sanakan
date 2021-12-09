using MySqlConnector;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class CardArenaStatsEnumerator : TableEnumerator<CardArenaStats>
    {
        public CardArenaStatsEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override CardArenaStats Current => new CardArenaStats
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.CardArenaStats);
    }
}
