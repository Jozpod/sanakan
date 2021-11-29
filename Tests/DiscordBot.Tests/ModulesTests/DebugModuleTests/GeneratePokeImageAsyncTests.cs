using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Sanakan.Configuration;
using Sanakan.Common.Configuration;
using System;
using System.Collections.Generic;
using Sanakan.Game.Models;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.GeneratePokeImageAsync(int)"/> method.
    /// </summary>
    [TestClass]
    public class GeneratePokeImageAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Generate_Image()
        {
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.E, Dere.Bodere, DateTime.UtcNow);
            var safariImage = new SafariImage();
            var safariImages = new List<SafariImage>()
            {
                safariImage,
            };
            var characterInfoResult = new Result<CharacterInfo>
            {
                Value = new CharacterInfo
                {
                    PictureId = 1ul,
                }
            };
            var imageUrl = "https://test.com";

            _resourceManagerMock
                .Setup(pr => pr.ReadFromJsonAsync<List<SafariImage>>(It.IsAny<string>()))
                .ReturnsAsync(safariImages);

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(2))
                .ReturnsAsync(characterInfoResult);

            _waifuServiceMock
               .Setup(pr => pr.GenerateNewCard(null, characterInfoResult.Value))
               .Returns(card)
               .Verifiable();

            _waifuServiceMock
                .Setup(pr => pr.GetSafariViewAsync(safariImage, card, _textChannelMock.Object))
                .ReturnsAsync(imageUrl)
                .Verifiable();

            var imageIndex = 0;
            await _module.GeneratePokeImageAsync(imageIndex);
        }
    }
}
