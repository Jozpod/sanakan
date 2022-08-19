using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class CardPvPStatsEnumerator : TableEnumerator<CardPvPStats>
    {
        public CardPvPStatsEnumerator(IDbConnection connection)
            : base(connection)
        {
        }

        public override CardPvPStats Current => new ()
        {
            Id = _reader.GetUInt64(0),
            Type = (FightType)_reader.GetInt32(1),
            Result = (FightResult)_reader.GetInt32(2),
            GameDeckId = _reader.GetUInt64(3),
        };

        public override string TableName => nameof(SanakanDbContext.CardPvPStats);
    }
}
