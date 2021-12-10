using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class CardArenaStatsEnumerator : TableEnumerator<CardArenaStats>
    {
        public CardArenaStatsEnumerator(IDbConnection connection)
            : base(connection) { }

        public override CardArenaStats Current => new()
        {
            Id = _reader.GetUInt64(0),
            WinsCount = _reader.GetInt64(1),
            DrawsCount = _reader.GetInt64(2),
            LosesCount = _reader.GetInt64(3),
            CardId = _reader.GetUInt64(4),
        };

        public override string TableName => nameof(SanakanDbContext.CardArenaStats);
    }
}
