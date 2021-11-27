using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using System;
using System.Threading.Tasks;
using Sanakan.Web.Controllers;

namespace Sanakan.Web.Tests.Controllers.QuizControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="QuizController.GetQuestionsAsync"/> method.
    /// </summary>
    [TestClass]
    public class GetQuestionAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Question()
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
                .Setup(pr => pr.GetCachedQuestionAsync(question.Id))
                .ReturnsAsync(question);

            var result = await _controller.GetQuestionAsync(question.Id);
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().BeEquivalentTo(question);
        }
    }
}
