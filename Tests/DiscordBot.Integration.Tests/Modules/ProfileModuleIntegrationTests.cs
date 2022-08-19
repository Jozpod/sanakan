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
    public class ProfileModuleIntegrationTests : TestBase
    {
        [TestMethod]
        public async Task TC601_Should_Show_Roles()
        {
            var commandMessage = ProfileCommandBuilder.ShowRoles(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC602_Should_Show_HowMuchToLevelUp()
        {
            var commandMessage = ProfileCommandBuilder.ShowHowMuchToLevelUp(prefix, FakeUser);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC603_Should_Show_Wallet()
        {
            var commandMessage = ProfileCommandBuilder.ShowWallet(prefix, FakeUser);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }
    }
}
