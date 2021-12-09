using MySqlConnector;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class WaifuCommandChannelEnumerator : TableEnumerator<WaifuCommandChannel>
    {
        public WaifuCommandChannelEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override WaifuCommandChannel Current => new WaifuCommandChannel
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.WaifuCommandChannels);
    }
}
