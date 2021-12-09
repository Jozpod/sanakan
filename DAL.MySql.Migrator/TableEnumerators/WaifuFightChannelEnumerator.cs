using MySqlConnector;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class WaifuFightChannelEnumerator : TableEnumerator<WaifuFightChannel>
    {
        public WaifuFightChannelEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override WaifuFightChannel Current => new WaifuFightChannel
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.WaifuFightChannels);
    }
}
