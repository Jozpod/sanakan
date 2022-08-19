using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Integration.Tests.CommandBuilders;
using Sanakan.Game.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Integration.Tests
{
#if DEBUG
    [TestClass]
#endif
    public class FunModuleIntegrationTests : TestBase
    {
        [TestMethod]
        public async Task TC001_Should_Receive_Daily_Sc_Coins()
        {
            var commandMessage = FunCommandBuilder.GiveDailySc(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC002_Should_Receive_Hourly_Sc_Coins()
        {
            var commandMessage = FunCommandBuilder.GiveHourlySc(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC003_Should_Mute_Decline()
        {
            var commandMessage = FunCommandBuilder.GiveMute(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC004_Should_Toss_Coin()
        {
            var commandMessage = FunCommandBuilder.TossCoin(prefix, CoinSide.Head, 1);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC005_Should_Play_On_Slot_Machine()
        {
            var commandMessage = FunCommandBuilder.PlayOnSlotMachine(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC006_Should_Giver_User_Sc()
        {
            var commandMessage = FunCommandBuilder.GiveUserSc(prefix, BotUser, 1000);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }
    }
}
