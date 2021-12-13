using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http.Json;
using Sanakan.Web.Controllers;
using System.Threading.Tasks;
using static Sanakan.Web.ResponseExtensions;
using Sanakan.DAL.Models;

namespace Sanakan.Web.Tests.IntegrationTests
{
    /// <summary>
    /// Defines tests for <see cref="QuizController"/> class.
    /// </summary>
    public partial class TestBase
    {
        /// <summary>
        /// Defines test for <see cref="QuizController.AddQuestionAsync(Question)"/> method.
        /// </summary>
        [TestMethod]
        public async Task Should_Add_Question()
        {
            await AuthorizeAsync();

            var question = new Question
            {
                Content = "test",
                PointsLose = 10,
                PointsWin = 20,
                TimeToAnswer = TimeSpan.FromMinutes(5),
            };
            var answer1 = new Answer
            {
                Content = "answer 1",
                Number = 1,
            };
            var answer2 = new Answer
            {
                Content = "answer 2",
                Number = 2,
            };
            question.Answers.Add(answer1);
            question.Answers.Add(answer2);
            var response = await _client.PostAsJsonAsync("api/quiz/question", question, _jsonSerializerOptions);
            var payload = await response.Content.ReadFromJsonAsync<ShindenPayload>();
            payload.Should().NotBeNull();
        }
    }
}
