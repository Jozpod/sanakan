using MySqlConnector;
using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class ReportsEnumerator : TableEnumerator<Report>
    {
        public ReportsEnumerator(MySqlConnection connection)
             : base(connection) { }

        public override Report Current => new Report
        {
            Id = _reader.GetUInt64(0),
            UserId = _reader.GetUInt64(1),
            MessageId = _reader.GetUInt64(2),
            GuildOptionsId = _reader.GetUInt64(3),
        };

        public override string TableName => nameof(SanakanDbContext.Raports);
    }
}
