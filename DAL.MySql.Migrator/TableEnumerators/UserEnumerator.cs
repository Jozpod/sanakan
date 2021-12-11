using Sanakan.Common;
using Sanakan.Common.Models;
using Sanakan.DAL.Models;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class UserEnumerator : TableEnumerator<User>
    {
        public UserEnumerator(IDbConnection connection)
            : base(connection) { }

        public override User Current
        {
            get
            {
                ulong? shindenId = _reader.IsDBNull(1) ? null : _reader.GetUInt64(1);
                shindenId = shindenId == 0 ? null : shindenId;

                var statsReplacementProfileUri = _reader.IsDBNull(10) ? null : _reader.GetString(10);
                statsReplacementProfileUri = statsReplacementProfileUri == "none" ? null : statsReplacementProfileUri;

                return new()
                {
                    Id = _reader.GetUInt64(0),
                    ShindenId = shindenId,
                    IsBlacklisted = _reader.GetBoolean(2),
                    AcCount = _reader.GetInt64(3),
                    TcCount = _reader.GetInt64(4),
                    ScCount = _reader.GetInt64(5),
                    Level = _reader.GetUInt64(6),
                    ExperienceCount = _reader.GetUInt64(7),
                    ProfileType = (ProfileType)_reader.GetUInt64(8),
                    BackgroundProfileUri = _reader.IsDBNull(9) ? Paths.DefaultBackgroundPicture : _reader.GetString(9),
                    StatsReplacementProfileUri = statsReplacementProfileUri,
                    MessagesCount = _reader.GetUInt64(11),
                    CommandsCount = _reader.GetUInt64(12),
                    MeasuredOn = _reader.GetDateTime(13),
                    MessagesCountAtDate = _reader.GetUInt64(14),
                    CharacterCountFromDate = _reader.GetUInt64(15),
                    ShowWaifuInProfile = _reader.GetBoolean(16),
                    WarningsCount = _reader.GetInt64(17),
                };
            }
        }

        public override string TableName => nameof(SanakanDbContext.Users);
    }
}
