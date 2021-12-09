using MySqlConnector;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class BoosterPackCharacterEnumerator : TableEnumerator<BoosterPackCharacter>
    {
        public BoosterPackCharacterEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override BoosterPackCharacter Current => new BoosterPackCharacter
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.BoosterPackCharacters);
    }
}
