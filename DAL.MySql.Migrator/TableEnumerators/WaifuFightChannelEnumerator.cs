using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class WaifuFightChannelEnumerator : TableEnumerator<WaifuFightChannel>
    {
        public WaifuFightChannelEnumerator(IDbConnection connection)
            : base(connection)
        {
        }

        public override WaifuFightChannel Current => new WaifuFightChannel
        {
            ChannelId = _reader.GetUInt64(1),
            WaifuId = _reader.GetUInt64(2),
        };

        public override string TableName => nameof(SanakanDbContext.WaifuFightChannels);
    }
}
