using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Integration.Tests.CommandBuilders;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.DiscordBot.Integration.Tests
{
#if DEBUG
    [TestClass]
#endif
    public class PocketWaifuModuleTests : TestBase
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
            var commandMessage = PocketWaifuCommandBuilder.ShowCardImage(Prefix, 5, false);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var attachment = message.Attachments.FirstOrDefault();
            attachment.Should().NotBeNull();
            attachment.Url.Should().NotBeNull();

            message = await WaitForMessageAsync();
            message.Should().NotBeNull();
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
            var characterInfoResult = new ShindenResult<CharacterInfo>
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
        public async Task TC407_Should_Get_Free_Card()
        {
            var characterId = 1488ul;
            var charactersResult = new ShindenResult<IEnumerable<ulong>>
            {
                Value = new List<ulong>
                {
                    characterId,
                }
            };
            var characterResult = new ShindenResult<CharacterInfo>
            {
                Value = new CharacterInfo
                {
                    Relations = new List<StaffInfoRelation>
                    {
                        new StaffInfoRelation
                        {
                            FirstName = "Giga",
                            LastName = "Chad",
                            Title = "Giga Chad",
                        }
                    }
                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetAllCharactersFromAnimeAsync())
                .ReturnsAsync(charactersResult);

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(characterId))
                .ReturnsAsync(characterResult);

            var commandMessage = PocketWaifuCommandBuilder.GetFreeCard(Prefix);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC408_Should_Update_Card()
        {
            var characterResult = new ShindenResult<CharacterInfo>
            {
                Value = new CharacterInfo
                {
                    Relations = new List<StaffInfoRelation>
                    {
                        new StaffInfoRelation
                        {
                            FirstName = "Giga",
                            LastName = "Chad",
                            Title = "Giga Chad",
                        }
                    }
                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(It.IsAny<ulong>()))
                .ReturnsAsync(characterResult);

            var commandMessage = PocketWaifuCommandBuilder.UpdateCard(Prefix, 5);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC409_Should_Open_Card_Bundle()
        {
            var characterId = 1ul;
            var charactersResult = new ShindenResult<IEnumerable<ulong>>
            {
                Value = new List<ulong>
                {
                    characterId
                }
            };
            var characterInfoResult = new Sanakan.ShindenApi.ShindenResult<CharacterInfo>
            {
                Value = new CharacterInfo
                {
                    FirstName = "Test",
                    LastName = "Character",
                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetAllCharactersFromAnimeAsync())
                .ReturnsAsync(charactersResult);

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(characterId))
                .ReturnsAsync(characterInfoResult);

            var commandMessage = PocketWaifuCommandBuilder.OpenPacket(Prefix, 1, checkWishlists: false);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC410_Should_Use_Item()
        {
            var characterId = 2ul;
            var characterInfo = new ShindenResult<CharacterInfo>();

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(characterId))
                .ReturnsAsync(characterInfo);

            var commandMessage = PocketWaifuCommandBuilder.UseItem(Prefix, 1, characterId);
            await Channel.SendMessageAsync(commandMessage);

            var message = await WaitForMessageAsync();
            message.Should().NotBeNull();
            var embed = message.Embeds.FirstOrDefault();
            embed.Should().NotBeNull();
        }

        [TestMethod]
        public async Task TC499_Should_Show_Cards()
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
