using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Sanakan.Web.Controllers;
using System.Collections.Generic;
using Sanakan.DAL.Models;
using System;
using Moq;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.GetCardsWithTagAsync(string)"/> method.
    /// </summary>
    [TestClass]
    public class GetCardsWithTagAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var tag = "tag";
            var cards = new List<Card>
            {
                new Card(1ul, "title", "name", 100, 50, Rarity.B, Dere.Bodere, DateTime.UtcNow),
            };

            _cardRepositoryMock
                .Setup(pr => pr.GetCardsWithTagAsync(tag))
                .ReturnsAsync(cards);

            var result = await _controller.GetCardsWithTagAsync(tag);
            var okObjectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okObjectResult.Value.Should().BeEquivalentTo(cards);
        }
    }
}
