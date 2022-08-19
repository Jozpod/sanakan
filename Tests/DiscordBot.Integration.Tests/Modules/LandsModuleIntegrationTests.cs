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
    public class LandsModuleIntegrationTests : TestBase
    {
        [TestMethod]
        public async Task TC201_Should_Show_People_In_Land()
        {
            var commandMessage = LandsCommandBuilder.ShowPeople(prefix);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC202_Should_Add_Person_To_Land()
        {
            var commandMessage = LandsCommandBuilder.AddPerson(prefix, FakeUser.Mention);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC203_Should_Remove_Person_From_Land()
        {
            var commandMessage = LandsCommandBuilder.RemovePerson(prefix, FakeUser.Mention);
            await channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }
    }
}
