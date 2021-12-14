using Discord;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.DiscordBot.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.ModulesTests.FunModuleTests
{
    /// <summary>
    /// Defines tests for <see cref="FunModule.ShowRiddleAsync"/> method.
    /// </summary>
    [TestClass]
    public class ShowRiddleAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Send_Message_With_Riddle()
        {
            var question = new Question()
            {
                Answers = new List<Answer>
                {
                    new Answer { Number = 1 },
                    new Answer { Number = 2 },
                    new Answer { Number = 3 },
                },
                AnswerNumber = 2,
            };
            var questions = new List<Question>
            {
                question,
            };
            var guildUserMock = new Mock<IGuildUser>(MockBehavior.Strict);
            var reactionUsers = new[]
            {
                new List<IUser>() { _userMock.Object },
            }.ToAsyncEnumerable();
            var emptyReactions = AsyncEnumerable.Empty<List<IUser>>();

            _userMock
                .Setup(pr => pr.Id)
                .Returns(1ul);

            _userMock
                .Setup(pr => pr.Mention)
                .Returns("user mention");

            _questionRepositoryMock
                .Setup(pr => pr.GetCachedAllQuestionsAsync())
                .ReturnsAsync(questions);

            _randomNumberGeneratorMock
                .Setup(pr => pr.Shuffle(It.IsAny<IEnumerable<Question>>()))
                .Returns(questions);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<int>>()))
                .Returns<IEnumerable<int>>((value) => value.First());

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetOneRandomFrom(It.IsAny<IEnumerable<Question>>()))
                .Returns<IEnumerable<Question>>((value) => value.First());

            _taskManagerMock
                .Setup(pr => pr.Delay(It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            _messageChannelMock
                .Setup(pr => pr.SendMessageAsync(
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<Embed>(),
                    It.IsAny<RequestOptions>(),
                    It.IsAny<AllowedMentions>(),
                    It.IsAny<MessageReference>()))
                .ReturnsAsync(_userMessageMock.Object);

            _userMessageMock
                .Setup(pr => pr.AddReactionAsync(It.IsAny<IEmote>(), null))
                .Returns(Task.CompletedTask);

            _userMessageMock
                .SetupSequence(pr => pr.GetReactionUsersAsync(It.IsAny<IEmote>(), 100, null))
                .Returns(reactionUsers)
                .Returns(reactionUsers)
                .Returns(emptyReactions)
                .Returns(emptyReactions);

            _userMessageMock
                .Setup(pr => pr.RemoveAllReactionsAsync(null))
                .Returns(Task.CompletedTask);

            await _module.ShowRiddleAsync();
        }
    }
}
