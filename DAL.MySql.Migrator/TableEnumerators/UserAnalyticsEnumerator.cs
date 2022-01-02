using Sanakan.DAL.Models.Analytics;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class UserAnalyticsEnumerator : TableEnumerator<UserAnalytics>
    {
        public UserAnalyticsEnumerator(IDbConnection connection)
            : base(connection)
        {
        }

        public override UserAnalytics Current
        {
            get
            {
                ulong? guildId = _reader.IsDBNull(3) ? null : _reader.GetUInt64(3);
                guildId = guildId == 0 ? null : guildId;

                return new UserAnalytics
                {
                    Id = _reader.GetUInt64(0),
                    Value = _reader.GetUInt64(1),
                    UserId = _reader.GetUInt64(2),
                    GuildId = guildId,
                    MeasuredOn = _reader.GetDateTime(4),
                    Type = (UserAnalyticsEventType)_reader.GetInt32(5),
                };
            }
        }

        public override string TableName => nameof(SanakanDbContext.UsersData);
    }
}
