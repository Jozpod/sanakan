using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Api.Models;
using Sanakan.Common;
using Sanakan.Common.Configuration;
using Sanakan.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.TokenControllerTests
{
    [TestClass]
    public class CreateTokenTests : Base
    {
        [TestMethod]
        public void Should_Return_Unauthorized()
        {
            var result = _controller.CreateToken(null);
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [TestMethod]
        public void Should_Return_Forbidden()
        {
            _apiConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new ApiConfiguration
                {
                    ApiKeys = new List<SanakanApiKey>(),
                });

            var result = _controller.CreateToken("test token");
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }

        [TestMethod]
        public void Should_Return_Token()
        {
            var apiKey = "test api key";

            _apiConfigurationMock
                .Setup(pr => pr.CurrentValue)
                .Returns(new ApiConfiguration
                {
                    ApiKeys = new List<SanakanApiKey>()
                    {
                        new SanakanApiKey
                        {
                            Bearer = "test bearer",
                            Key = apiKey,
                        }
                    }
                });

            var expected = new TokenData
            {
                Expire = DateTime.UtcNow,
                Token = "test token",
            };

            _jwtBuilderMock
                .Setup(pr => pr.Build(It.IsAny<TimeSpan>(), It.IsAny<Claim[]>()))
                .Returns(expected);

            var result = _controller.CreateToken(apiKey);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().BeEquivalentTo(expected);
        }
    }
}
