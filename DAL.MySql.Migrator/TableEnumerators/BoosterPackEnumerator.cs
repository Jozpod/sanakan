using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class BoosterPackEnumerator : TableEnumerator<BoosterPack>
    {
        public BoosterPackEnumerator(IDbConnection connection)
            : base(connection) { }

        public override BoosterPack Current => new()
        {
            Id = _reader.GetUInt64(0),
            Name = _reader.GetString(1),
            TitleId = _reader.GetUInt64(2),
            CardCount = _reader.GetUInt32(3),
            MinRarity = (Rarity)_reader.GetInt32(4),
            IsCardFromPackTradable = _reader.GetBoolean(5),
            CardSourceFromPack = (CardSource)_reader.GetInt32(6),
            GameDeckId = _reader.GetUInt64(7),
        };

        public override string TableName => nameof(SanakanDbContext.BoosterPacks);
    }
}
