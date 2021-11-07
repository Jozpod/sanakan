﻿using DiscordBot.Services.PocketWaifu.Abstractions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.QuizControllerTests
{
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
                    Answer = 1,
                    PointsWin = 10,
                    PointsLose = 10,
                    TimeToAnswer = TimeSpan.FromMinutes(1),
                }
            };

            _questionRepositoryMock
                .Setup(pr => pr.GetCachedAllQuestionsAsync())
                .ReturnsAsync(questions);

            var result = await _controller.GetQuestionsAsync();
            var okObjectResult = result.Should().BeOfType<ObjectResult>().Subject;
            okObjectResult.Value.Should().BeEquivalentTo(questions);
        }
    }
}
