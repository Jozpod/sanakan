using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class CommandChannelEnumerator : TableEnumerator<CommandChannel>
    {
        public CommandChannelEnumerator(IDbConnection connection)
          : base(connection) { }

        public override CommandChannel Current => new()
        {
            ChannelId = _reader.GetUInt64(1),
            GuildOptionsId = _reader.GetUInt64(2),
        };

        public override string TableName => nameof(SanakanDbContext.CommandChannels);
    }
}
