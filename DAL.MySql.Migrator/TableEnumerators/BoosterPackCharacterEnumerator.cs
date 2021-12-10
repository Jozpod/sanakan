using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class BoosterPackCharacterEnumerator : TableEnumerator<BoosterPackCharacter>
    {
        public BoosterPackCharacterEnumerator(IDbConnection connection)
            : base(connection) { }

        public override BoosterPackCharacter Current => new()
        {
            Id = _reader.GetUInt64(0),
            CharacterId = _reader.GetUInt64(1),
            BoosterPackId = _reader.GetUInt64(2),
        };

        public override string TableName => nameof(SanakanDbContext.BoosterPackCharacters);
    }
}
