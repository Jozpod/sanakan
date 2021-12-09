namespace Sanakan.DiscordBot.Tests.IntegrationTests
{
    public class DiscordIntegrationTestOptions
    {
        public string FakeUserBotToken { get; set; }

        public ulong GuildId { get; set; }

        public ulong MainChannelId { get; set; }

        public ulong MuteRoleId { get; set; }

        public ulong UserRoleId { get; set; }

        public ulong AdminRoleId { get; set; }
        
    }
}
