using MySqlConnector;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class WaifuConfigurationEnumerator : TableEnumerator<WaifuConfiguration>
    {
        public WaifuConfigurationEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override WaifuConfiguration Current => new WaifuConfiguration
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.Waifus);
    }
}
