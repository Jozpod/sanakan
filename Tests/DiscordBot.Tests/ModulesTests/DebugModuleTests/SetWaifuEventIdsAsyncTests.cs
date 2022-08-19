using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DiscordBot.Modules;
using Sanakan.DiscordBot.Services;
using System;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.SetWaifuEventIdsAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class SetWaifuEventIdsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Set_Event_Ids_And_Send_Reply()
        {
            var url = new Uri("https://test.com/text.txt");
            var result = new EventIdsImporterResult();

            _eventIdsImporterMock
                .Setup(pr => pr.RunAsync(url))
                .ReturnsAsync(result);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.SetWaifuEventIdsAsync(url);
        }
    }
}
