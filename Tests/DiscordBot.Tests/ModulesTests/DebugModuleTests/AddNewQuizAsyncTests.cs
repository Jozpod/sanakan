using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.AddNewQuizAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class AddNewQuizAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Add_Quiz_And_Send_Message()
        {
            var question = new Question()
            {
                Id = 1,
                Content = "test",
                Answers = new[]
                {
                    new Answer(),
                    new Answer(),
                }
            };
            var questionJson = JsonSerializer.Serialize(question);

            _questionRepositoryMock
                .Setup(pr => pr.Add(It.IsAny<Question>()));

            _questionRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.AddNewQuizAsync(questionJson);
        }
    }
}
