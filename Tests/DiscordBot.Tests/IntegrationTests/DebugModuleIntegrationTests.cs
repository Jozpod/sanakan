using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests.IntegrationTests
{
    public partial class TestBase
    {
        [TestMethod]
        public async Task TC701_Should_Change_User_Ac()
        {
            var commandMessage = DebugCommandBuilder.ChangeUserAc(Prefix, FakeUser.Mention, 1000);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC702_Should_Change_User_Ct()
        {
            var commandMessage = DebugCommandBuilder.ChangeUserCt(Prefix, FakeUser.Mention, 1000);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC703_Should_Change_User_Level()
        {
            var commandMessage = DebugCommandBuilder.ChangeUserLevel(Prefix, FakeUser.Mention, 1000);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC704_Should_Change_Card_Title()
        {
            var commandMessage = DebugCommandBuilder.ChangeTitleCard(Prefix, 1, "new title");
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }
    }
}
