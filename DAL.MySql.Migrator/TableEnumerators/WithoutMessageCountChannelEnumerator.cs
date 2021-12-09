using MySqlConnector;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class WithoutMessageCountChannelEnumerator : TableEnumerator<WithoutMessageCountChannel>
    {
        public WithoutMessageCountChannelEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override WithoutMessageCountChannel Current => new WithoutMessageCountChannel
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.IgnoredChannels);
    }
}
