using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.QuizControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="QuizController.GetQuestionAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GetQuestionsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Questions()
        {
            var questions = new List<Question>()
            {
                new Question
                {
                    Id = 1,
                    Content = "test",
                    AnswerNumber = 1,
                    PointsWin = 10,
                    PointsLose = 10,
                    TimeToAnswer = TimeSpan.FromMinutes(1),
                }
            };

            _questionRepositoryMock
                .Setup(pr => pr.GetCachedAllQuestionsAsync())
                .ReturnsAsync(questions);

            var result = await _controller.GetQuestionsAsync();
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().BeEquivalentTo(questions);
        }
    }
}
