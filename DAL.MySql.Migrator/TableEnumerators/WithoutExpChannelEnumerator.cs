using MySqlConnector;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class WithoutExpChannelEnumerator : TableEnumerator<WithoutExpChannel>
    {
        public WithoutExpChannelEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override WithoutExpChannel Current => new WithoutExpChannel
        {
            Id = _reader.GetUInt64(0),
        };

        public override string TableName => nameof(SanakanDbContext.WithoutExpChannels);
    }
}
