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
    public class ModerationModuleIntegrationTests : TestBase
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

        [TestMethod]
        public async Task TC506_Should_Set_Nsfw_Channel()
        {
            var commandMessage = ModerationCommandBuilder.SetNsfwChannel(Prefix);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC507_Should_Set_Market_Waifu_Channel()
        {
            var commandMessage = ModerationCommandBuilder.SetMarketWaifuChannel(Prefix);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC508_Should_Check_User()
        {
            var commandMessage = ModerationCommandBuilder.CheckUser(Prefix, FakeUser.Mention);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC509_Should_Start_Lottery()
        {
            var commandMessage = ModerationCommandBuilder.GetRandomUser(Prefix, 1);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();

            message = await WaitForMessageAsync();
            message.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC510_Should_Resolve_Report()
        {
            var commandMessage = ModerationCommandBuilder.ResolveReport(Prefix, 1);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC520_Should_Ban_User()
        {
            //var commandMessage = ModerationCommandBuilder.BanUser(Prefix, FakeUser.Mention, "00:00:05");
            //await Channel.SendMessageAsync(commandMessage);

            //var message = await WaitForMessageAsync();
            //message.Should().NotBeNull();
            //var embed = message.Embeds.FirstOrDefault();
            //embed.Should().NotBeNull();

            // https://discord.com/oauth2/authorize?client_id=911409545094512671&permissions=8&scope=applications.commands%20bot
        }

    }
}
