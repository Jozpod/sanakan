using MySqlConnector;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class BoosterPackEnumerator : TableEnumerator<BoosterPack>
    {
        public BoosterPackEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override BoosterPack Current => new BoosterPack
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.BoosterPacks);
    }
}
