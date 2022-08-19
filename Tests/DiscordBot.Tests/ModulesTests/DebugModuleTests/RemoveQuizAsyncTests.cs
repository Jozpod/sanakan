using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.DebugModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="DebugModule.RemoveQuizAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class RemoveQuizAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Error_Message_No_Quiz()
        {
            _questionRepositoryMock
                .Setup(pr => pr.GetByIdAsync(1))
                .ReturnsAsync(null as Question);

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.RemoveQuizAsync(1);
        }

        [TestMethod]
        public async Task Should_Remove_Quiz_And_Send_Confirm_Message()
        {
            var question = new Question();

            _questionRepositoryMock
                .Setup(pr => pr.GetByIdAsync(question.Id))
                .ReturnsAsync(question);

            _questionRepositoryMock
                .Setup(pr => pr.Remove(It.IsAny<Question>()));

            _questionRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
                .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            SetupSendMessage((message, embed) =>
            {
                embed.Should().NotBeNull();
                embed.Description.Should().NotBeNullOrEmpty();
            });

            await _module.RemoveQuizAsync(question.Id);
        }
    }
}
