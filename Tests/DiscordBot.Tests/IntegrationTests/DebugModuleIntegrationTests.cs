using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests.IntegrationTests
{
    public partial class TestBase
    {
        [TestMethod]
        public async Task Should_Run()
        {
            var guild = await _client.GetGuildAsync(_configuration.MainGuild);
            var mainChannel = 910284207534796800ul;

            var textChannel = await guild.GetTextChannelAsync(mainChannel);

            var message = ".ktoto <@!363676687667429376>";
            await textChannel.SendMessageAsync(".ktoto @Jotpe#5246");
            //await textChannel.SendMessageAsync(".info");

            await Task.Delay(Timeout.InfiniteTimeSpan);
        }
    }
}
