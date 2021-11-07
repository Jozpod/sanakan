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
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.QuizControllerTests
{
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
                Answer = 1,
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