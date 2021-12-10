using Sanakan.DAL.Models.Configuration;

namespace Sanakan.DAL.MySql.Migrator.TableEnumerators
{
    public class GuildOptionsEnumerator : TableEnumerator<GuildOptions>
    {
        public GuildOptionsEnumerator(IDbConnection connection)
           : base(connection) { }

        public override GuildOptions Current => new()
        {
            Id = _reader.GetUInt64(0),
            MuteRoleId = _reader.GetUInt64(1),
            ModMuteRoleId = _reader.GetUInt64(2),
            UserRoleId = _reader.GetUInt64(3),
            AdminRoleId = _reader.GetUInt64(4),
            GlobalEmotesRoleId = _reader.GetUInt64(5),
            WaifuRoleId = _reader.GetUInt64(6),
            NotificationChannelId = _reader.GetUInt64(7),
            RaportChannelId = _reader.GetUInt64(8),
            QuizChannelId = _reader.GetUInt64(9),
            ToDoChannelId = _reader.GetUInt64(10),
            NsfwChannelId = _reader.GetUInt64(11),
            LogChannelId = _reader.GetUInt64(12),
            GreetingChannelId = _reader.GetUInt64(13),
            WelcomeMessage = _reader.GetString(14),
            WelcomeMessagePM = _reader.GetString(15),
            GoodbyeMessage = _reader.GetString(16),
            SafariLimit = _reader.GetUInt64(17),
            SupervisionEnabled = _reader.GetBoolean(18),
            ChaosModeEnabled = _reader.GetBoolean(19),
            Prefix = _reader.GetString(20),
        };

        public override string TableName => nameof(SanakanDbContext.Guilds);
    }
}
