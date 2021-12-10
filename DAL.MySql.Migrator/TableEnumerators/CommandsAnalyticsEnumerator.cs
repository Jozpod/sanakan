using Sanakan.DAL.Models.Analytics;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class CommandsAnalyticsEnumerator : TableEnumerator<CommandsAnalytics>
    {
        public CommandsAnalyticsEnumerator(IDbConnection connection)
          : base(connection) { }

        public override CommandsAnalytics Current => new()
        {
            Id = _reader.GetUInt64(0),
            UserId = _reader.GetUInt64(1),
            GuildId = _reader.GetUInt64(2),
            CreatedOn = _reader.GetDateTime(3),
            CommandName = _reader.GetString(4),
            CommandParameters = _reader.GetString(5),
        };

        public override string TableName => nameof(SanakanDbContext.CommandsData);
    }
}
