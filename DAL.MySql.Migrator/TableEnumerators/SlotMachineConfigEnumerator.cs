using MySqlConnector;
using Sanakan.DAL.Models;
using Sanakan.DAL.Models.Analytics;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class SlotMachineConfigEnumerator : TableEnumerator<SlotMachineConfig>
    {
        public SlotMachineConfigEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override SlotMachineConfig Current => new SlotMachineConfig
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.SlotMachineConfigs);
    }
}
