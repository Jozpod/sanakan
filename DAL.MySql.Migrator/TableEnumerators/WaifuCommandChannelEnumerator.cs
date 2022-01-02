using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class WaifuCommandChannelEnumerator : TableEnumerator<WaifuCommandChannel>
    {
        public WaifuCommandChannelEnumerator(IDbConnection connection)
            : base(connection)
        {
        }

        public override WaifuCommandChannel Current => new()
        {
            ChannelId = _reader.GetUInt64(1),
            WaifuId = _reader.GetUInt64(2),
        };

        public override string TableName => nameof(SanakanDbContext.WaifuCommandChannels);
    }
}
