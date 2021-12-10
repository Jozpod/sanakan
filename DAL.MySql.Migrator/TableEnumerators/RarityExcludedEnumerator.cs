using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class RarityExcludedEnumerator : TableEnumerator<RarityExcluded>
    {
        public RarityExcludedEnumerator(IDbConnection connection)
            : base(connection) { }

        public override RarityExcluded Current => new()
        {
            Id = _reader.GetUInt64(0),
            Rarity = (Rarity)_reader.GetInt32(1),
            BoosterPackId = _reader.GetUInt64(2),
        };

        public override string TableName => nameof(SanakanDbContext.RaritysExcludedFromPacks);
    }
}
