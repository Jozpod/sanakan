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
        public async Task TC01_Should_Set_Admin_Role()
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
        public async Task TC02_Should_Set_Report_Channel()
        {
            var commandMessage = $"{Prefix}raportch";
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
            embed.Fields.Should().HaveCount(7);
        }

        [TestMethod]
        public async Task TC03_Should_Set_Log_Channel()
        {
            var commandMessage = $"{Prefix}logch";
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
            embed.Fields.Should().HaveCount(7);
        }
    }
}
