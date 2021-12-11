using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Tests.IntegrationTests
{
    public partial class TestBase
    {
        [TestMethod]
        public async Task TC401_Should_Send_Card_To_Expedition()
        {
            var commandMessage = PocketWaifuCommandBuilder.SendCardToExpedition(Prefix, 3, "n");
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC402_Should_Show_Items()
        {
            var commandMessage = PocketWaifuCommandBuilder.ShowItems(Prefix, 1);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC403_Should_Show_Card_Image()
        {
            var commandMessage = PocketWaifuCommandBuilder.ShowCardImage(Prefix, 1, false);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC404_Should_Destroy_Card()
        {
            var commandMessage = PocketWaifuCommandBuilder.DestroyCard(Prefix, 2);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC405_Should_Add_To_WishList()
        {
            var objectId = 1ul;
            var characterInfoResult = new Result<CharacterInfo>
            {
                Value = new CharacterInfo
                {
                    FirstName = "test",
                    LastName = "character",
                },
            };

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(objectId))
                .ReturnsAsync(characterInfoResult);

            var commandMessage = PocketWaifuCommandBuilder.AddToWishlist(Prefix, "postac", objectId);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC406_Should_Start_Craft_Session()
        {
            var commandMessage = PocketWaifuCommandBuilder.CraftCard(Prefix);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC407_Should_Show_Cards()
        {
            var commandMessage = PocketWaifuCommandBuilder.ShowCards(Prefix, "rarity");
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }
    }
}
