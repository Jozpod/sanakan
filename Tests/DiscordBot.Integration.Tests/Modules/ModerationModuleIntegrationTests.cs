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
            var adminRole = guild.Roles.FirstOrDefault(pr => pr.Name == "AdminRole").Id;
            var commandMessage = ModerationCommandBuilder.SetAdminRole(prefix, adminRole);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC502_Should_Set_Report_Channel()
        {
            var commandMessage = ModerationCommandBuilder.SetRaportChannel(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC503_Should_Set_Log_Channel()
        {
            var commandMessage = ModerationCommandBuilder.SetLogChannel(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC504_Should_Set_Greeting_Channel()
        {
            var commandMessage = ModerationCommandBuilder.SetGreetingChannel(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC505_Should_Set_Command_Waifu_Channel()
        {
            var commandMessage = ModerationCommandBuilder.SetCommandWaifuChannel(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC506_Should_Set_Nsfw_Channel()
        {
            var commandMessage = ModerationCommandBuilder.SetNsfwChannel(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC507_Should_Set_Market_Waifu_Channel()
        {
            var commandMessage = ModerationCommandBuilder.SetMarketWaifuChannel(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC508_Should_Check_User()
        {
            var commandMessage = ModerationCommandBuilder.CheckUser(prefix, FakeUser.Mention);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC509_Should_Start_Lottery()
        {
            var commandMessage = ModerationCommandBuilder.GetRandomUser(prefix, 1);
            await channel.SendMessageAsync(commandMessage);

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
            var commandMessage = ModerationCommandBuilder.ResolveReport(prefix, 1);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        // [TestMethod]
        // public async Task TC520_Should_Ban_User()
        // {
        //     var commandMessage = ModerationCommandBuilder.BanUser(Prefix, FakeUser.Mention, "00:00:05");
        //     await Channel.SendMessageAsync(commandMessage);

        //     var message = await WaitForMessageAsync();
        //     message.Should().NotBeNull();
        //     var embed = message.Embeds.FirstOrDefault();
        //     embed.Should().NotBeNull();
        // }
    }
}
