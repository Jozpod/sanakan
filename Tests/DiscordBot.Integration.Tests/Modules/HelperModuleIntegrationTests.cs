using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Integration.Tests.CommandBuilders;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Integration.Tests
{
#if DEBUG
    [TestClass]
#endif
    public class HelperModuleIntegrationTests : TestBase
    {
        [TestMethod]
        public async Task TC101_Should_Return_Latency()
        {
            var commandMessage = HelperCommandBuilder.GetPing(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
            embed.Description.Should().Contain("Pong");
        }

        [TestMethod]
        public async Task TC102_Should_Return_Bot_Info()
        {
            var commandMessage = HelperCommandBuilder.GiveBotInfo(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            message.Content.Should().Contain("Czas działania");
        }

        [TestMethod]
        public async Task TC103_Should_Return_User_Info()
        {
            var commandMessage = HelperCommandBuilder.GiveUserInfo(prefix, FakeUser.Mention);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
            embed.Fields.Should().HaveCount(7);
        }

        [TestMethod]
        public async Task TC104_Should_Return_Server_Info()
        {
            var commandMessage = HelperCommandBuilder.GetServerInfo(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
            embed.Fields.Should().HaveCount(7);
        }

        //[TestMethod]
        //public async Task Should_Report_User()
        //{
        //    var commandMessage = $"{Prefix}report <@!{FakeUserId}> test";
        //    await Channel.SendMessageAsync(commandMessage);
        //}
    }
}
