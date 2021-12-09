using MySqlConnector;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class RarityExcludedEnumerator : TableEnumerator<RarityExcluded>
    {
        public RarityExcludedEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override RarityExcluded Current => new RarityExcluded
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.RaritysExcludedFromPacks);
    }
}
