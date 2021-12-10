using MySqlConnector;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class WithoutMessageCountChannelEnumerator : TableEnumerator<WithoutMessageCountChannel>
    {
        public WithoutMessageCountChannelEnumerator(IDbConnection connection)
            : base(connection) { }

        public override WithoutMessageCountChannel Current => new()
        {
            ChannelId = _reader.GetUInt64(1),
            GuildOptionsId = _reader.GetUInt64(2),
        };

        public override string TableName => nameof(SanakanDbContext.IgnoredChannels);
    }
}
