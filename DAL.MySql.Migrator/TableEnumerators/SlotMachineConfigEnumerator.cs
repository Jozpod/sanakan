using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class SlotMachineConfigEnumerator : TableEnumerator<SlotMachineConfig>
    {
        public SlotMachineConfigEnumerator(IDbConnection connection)
            : base(connection) { }

        public override SlotMachineConfig Current => new SlotMachineConfig
        {
            Id = _reader.GetUInt64(0),
            PsayMode = _reader.GetInt64(1),
            Beat = (SlotMachineBeat)_reader.GetInt32(2),
            Rows = (SlotMachineSelectedRows)_reader.GetInt32(3),
            Multiplier = (SlotMachineBeatMultiplier)_reader.GetInt32(4),
            UserId = _reader.GetUInt64(5),
        };

        public override string TableName => nameof(SanakanDbContext.SlotMachineConfigs);
    }
}
