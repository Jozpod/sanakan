using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Session;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.ShindenModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="ShindenModule.SearchCharacterAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class SearchCharacterAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Error_Message_No_Character()
        {
            var userId = 1ul;
            var characterName = "test";
            var characterId = 1ul;
            var utcNow = DateTime.UtcNow;
            var searchCharacterResult = new ShindenResult<List<CharacterSearchResult>>();
            searchCharacterResult.StatusCode = System.Net.HttpStatusCode.NotFound;

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

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
            });

            await _module.SearchCharacterAsync(characterName);
        }

        [TestMethod]
        public async Task Should_Return_Character_Info_First()
        {
            var userId = 1ul;
            var characterName = "test";
            var characterId = 1ul;
            var utcNow = DateTime.UtcNow;
            var searchCharacterResult = new ShindenResult<List<CharacterSearchResult>>
            {
                Value = new List<CharacterSearchResult>
                {
                    new CharacterSearchResult
                    {
                        Id = characterId,
                        FirstName = "Test 1",
                    }
                }
            };
            var characterInfoResult = new ShindenResult<CharacterInfo>
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

        [TestMethod]
        public async Task Should_Split_Text()
        {
            var userId = 1ul;
            var characterName = "test";
            var characterId = 1ul;
            var utcNow = DateTime.UtcNow;
            var searchCharacterResult = new ShindenResult<List<CharacterSearchResult>>
            {
                Value = Enumerable.Range(0, 100).Select(pr => new CharacterSearchResult
                {
                    Id = (ulong)pr,
                    FirstName = "First name",
                    LastName = $"Last name {pr}",
                }).ToList(),
            };
            var characterInfoResult = new ShindenResult<CharacterInfo>
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

            _sessionManagerMock
                .Setup(pr => pr.Add(It.IsAny<SearchSession>()));

            SetupSendMessage();

            await _module.SearchCharacterAsync(characterName);
        }

        [TestMethod]
        public async Task Should_Start_Session()
        {
            var userId = 1ul;
            var characterName = "test";
            var characterId = 1ul;
            var utcNow = DateTime.UtcNow;
            var searchCharacterResult = new ShindenResult<List<CharacterSearchResult>>
            {
                Value = new List<CharacterSearchResult>
                {
                    new CharacterSearchResult
                    {
                        Id = characterId,
                        FirstName = "Test 1",
                    },
                     new CharacterSearchResult
                    {
                        Id = 2ul,
                        FirstName = "Test 2",
                    }
                }
            };
            var characterInfoResult = new ShindenResult<CharacterInfo>
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

            _sessionManagerMock
                .Setup(pr => pr.Add(It.IsAny<SearchSession>()));

            SetupSendMessage();

            await _module.SearchCharacterAsync(characterName);
        }
    }
}
