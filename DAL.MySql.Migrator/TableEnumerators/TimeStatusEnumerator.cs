using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class TimeStatusEnumerator : TableEnumerator<TimeStatus>
    {
        public TimeStatusEnumerator(IDbConnection connection)
            : base(connection) { }

        public override TimeStatus Current => new()
        {
            Id = _reader.GetUInt64(0),
            Type = (StatusType)_reader.GetInt32(1),
            EndsOn = _reader.IsDBNull(2) ? null : _reader.GetDateTime(2),
            IntegerValue = _reader.GetUInt64(3),
            BooleanValue = _reader.GetBoolean(4),
            GuildId = _reader.IsDBNull(5) ? null : _reader.GetUInt64(5),
            UserId = _reader.GetUInt64(6),
        };

        public override string TableName => nameof(SanakanDbContext.TimeStatuses);
    }
}
