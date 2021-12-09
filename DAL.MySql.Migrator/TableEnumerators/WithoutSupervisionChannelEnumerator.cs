using MySqlConnector;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class WithoutSupervisionChannelEnumerator : TableEnumerator<WithoutSupervisionChannel>
    {
        public WithoutSupervisionChannelEnumerator(MySqlConnection connection)
          : base(connection) { }

        public override WithoutSupervisionChannel Current => new WithoutSupervisionChannel
        {
            Id = _reader.GetUInt64(0),
            ChannelId = _reader.GetUInt64(1),
            GuildOptionsId = _reader.GetUInt64(2),
        };

        public override string TableName => nameof(SanakanDbContext.WithoutSupervisionChannels);
    }
}
