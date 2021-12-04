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
        public async Task TC501_Should_Set_Admin_Role()
        {
            var adminRole = Guild.Roles.FirstOrDefault(pr => pr.Name == "AdminRole").Id;
            var commandMessage = ModerationCommandBuilder.SetAdminRole(Prefix, adminRole);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
            embed.Fields.Should().HaveCount(7);
        }

        [TestMethod]
        public async Task TC502_Should_Set_Report_Channel()
        {
            var commandMessage = ModerationCommandBuilder.SetRaportChannel(Prefix);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC503_Should_Set_Log_Channel()
        {
            var commandMessage = ModerationCommandBuilder.SetLogChannel(Prefix);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC504_Should_Set_Greeting_Channel()
        {
            var commandMessage = ModerationCommandBuilder.SetGreetingChannel(Prefix);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC505_Should_Set_Command_Waifu_Channel()
        {
            var commandMessage = ModerationCommandBuilder.SetCommandWaifuChannel(Prefix);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }
    }
}
