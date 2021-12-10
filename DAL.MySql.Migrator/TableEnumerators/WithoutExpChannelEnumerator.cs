using MySqlConnector;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class WithoutExpChannelEnumerator : TableEnumerator<WithoutExpChannel>
    {
        public WithoutExpChannelEnumerator(IDbConnection connection)
            : base(connection) { }

        public override WithoutExpChannel Current => new()
        {
            ChannelId = _reader.GetUInt64(1),
            GuildOptionsId = _reader.GetUInt64(2),
        };

        public override string TableName => nameof(SanakanDbContext.WithoutExpChannels);
    }
}
