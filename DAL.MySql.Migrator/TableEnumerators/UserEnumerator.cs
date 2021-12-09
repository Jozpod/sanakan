using MySqlConnector;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class UserEnumerator : TableEnumerator<User>
    {
        public UserEnumerator(MySqlConnection connection)
            : base(connection) { }

        public override User Current => new User
        {
            Id = _reader.GetUInt64(0),
            ShindenId = _reader.GetUInt64(1),
            IsBlacklisted = _reader.GetBoolean(2),
            AcCount = _reader.GetInt64(3),
            TcCount = _reader.GetInt64(4),
            ScCount = _reader.GetInt64(5),
            Level = _reader.GetUInt64(6),
            ExperienceCount = _reader.GetUInt64(7),
            ProfileType = (ProfileType)_reader.GetUInt64(8),
            BackgroundProfileUri = _reader.GetString(9),
            StatsReplacementProfileUri = _reader.GetString(10),
            MessagesCount = _reader.GetUInt64(11),
            CommandsCount = _reader.GetUInt64(12),
            MeasuredOn = _reader.GetDateTime(13),
            MessagesCountAtDate = _reader.GetUInt64(14),
            CharacterCountFromDate = _reader.GetUInt64(15),
            ShowWaifuInProfile = _reader.GetBoolean(16),
            WarningsCount = _reader.GetInt64(17),
        };

        public override string TableName => nameof(SanakanDbContext.Users);
    }
}
