using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Web.Controllers;
using System;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.QuizControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="QuizController.RemoveQuestionAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class RemoveQuestionAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Remove_Question_And_Return_Ok()
        {
            var question = new Question
            {
                Id = 1,
                Content = "test",
                AnswerNumber = 1,
                PointsWin = 10,
                PointsLose = 10,
                TimeToAnswer = TimeSpan.FromMinutes(1),
            };

            _questionRepositoryMock
                .Setup(pr => pr.GetByIdAsync(question.Id))
                .ReturnsAsync(question);

            _questionRepositoryMock
                .Setup(pr => pr.Remove(question));

            _questionRepositoryMock
                .Setup(pr => pr.SaveChangesAsync(default))
                .Returns(Task.CompletedTask);

            _cacheManagerMock
               .Setup(pr => pr.ExpireTag(It.IsAny<string[]>()));

            var result = await _controller.RemoveQuestionAsync(question.Id);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().NotBeNull();
        }
    }
}
