using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.DiscordBot.Modules;
using Discord;
using Moq;
using System;
using Sanakan.DiscordBot.Session;
using System.Collections.Generic;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi;
using FluentAssertions;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenModule.SearchCharacterAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class SearchCharacterAsyncTests : Base
    {
        
        [TestMethod]
        public async Task Should_Return_Character_Info_First()
        {
            var userId = 1ul;
            var characterName = "test";
            var characterId = 1ul;
            var utcNow = DateTime.UtcNow;
            var searchCharacterResult = new Result<List<CharacterSearchResult>>
            {
                Value = new List<CharacterSearchResult>
                {
                    new CharacterSearchResult
                    {
                        Id = characterId,
                    }
                }
            };
            var characterInfoResult = new Result<CharacterInfo>
            {
                Value = new CharacterInfo
                {

                }
            };

            _sessionManagerMock
                 .Setup(pr => pr.Exists<SearchSession>(userId))
                 .Returns(false);

            _userMock
                .Setup(pr => pr.Id)
                .Returns(userId);

            _systemClockMock
                .Setup(pr => pr.UtcNow)
                .Returns(utcNow);

            _shindenClientMock
                .Setup(pr => pr.SearchCharacterAsync(characterName))
                .ReturnsAsync(searchCharacterResult);

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(characterId))
                .ReturnsAsync(characterInfoResult);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.SearchCharacterAsync(characterName);
        }
    }
}
