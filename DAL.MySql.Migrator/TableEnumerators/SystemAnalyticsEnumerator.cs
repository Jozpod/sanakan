using Sanakan.DAL.Models.Analytics;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class SystemAnalyticsEnumerator : TableEnumerator<SystemAnalytics>
    {
        public SystemAnalyticsEnumerator(IDbConnection connection)
            : base(connection)
        {
        }

        public override SystemAnalytics Current => new()
        {
            Id = _reader.GetUInt64(0),
            Value = _reader.GetInt64(1),
            MeasuredOn = _reader.GetDateTime(2),
            Type = (SystemAnalyticsEventType)_reader.GetInt32(3),
        };

        public override string TableName => nameof(SanakanDbContext.SystemData);
    }
}
