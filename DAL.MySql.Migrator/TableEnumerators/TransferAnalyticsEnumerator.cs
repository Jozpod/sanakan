using Sanakan.DAL.Models.Analytics;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class TransferAnalyticsEnumerator : TableEnumerator<TransferAnalytics>
    {
        public TransferAnalyticsEnumerator(IDbConnection connection)
            : base(connection)
        {
        }

        public override TransferAnalytics Current => new()
        {
            Id = _reader.GetUInt64(0),
            Value = (ulong)_reader.GetInt64(1),
            CreatedOn = _reader.GetDateTime(2),
            DiscordUserId = _reader.GetUInt64(3),
            ShindenUserId = _reader.GetUInt64(4),
            Source = (TransferSource)_reader.GetInt32(5),
        };

        public override string TableName => nameof(SanakanDbContext.TransferData);
    }
}
