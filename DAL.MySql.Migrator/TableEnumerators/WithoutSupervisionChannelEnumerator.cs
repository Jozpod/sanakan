using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class WithoutSupervisionChannelEnumerator : TableEnumerator<WithoutSupervisionChannel>
    {
        public WithoutSupervisionChannelEnumerator(IDbConnection connection)
          : base(connection)
        {
        }

        public override WithoutSupervisionChannel Current => new()
        {
            ChannelId = _reader.GetUInt64(1),
            GuildOptionsId = _reader.GetUInt64(2),
        };

        public override string TableName => nameof(SanakanDbContext.WithoutSupervisionChannels);
    }
}
