using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Web.Controllers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.GetCardAsync(ulong)"/> method.
    /// </summary>
    [TestClass]
    public class GetCardAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Forbidden()
        {
            var cardId = 1ul;

            _fileSystemMock
                .Setup(pr => pr.Exists(It.IsAny<string>()))
                .Returns(true);

            var result = await _controller.GetCardAsync(cardId);
            var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
            objectResult.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        }

        [TestMethod]
        public async Task Should_Return_Ok_With_Image_Payload()
        {
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.B, Dere.Bodere, DateTime.UtcNow);

            _fileSystemMock
                .Setup(pr => pr.Exists(It.IsAny<string>()))
                .Returns(false);

            _cardRepositoryMock
                .Setup(pr => pr.GetByIdAsync(card.Id))
                .ReturnsAsync(card);

            _waifuServiceMock
                .Setup(pr => pr.DeleteCardImageIfExist(card));

            _waifuServiceMock
               .Setup(pr => pr.GenerateAndSaveCardAsync(It.IsAny<Card>(), CardImageType.Normal))
               .ReturnsAsync("./image/path");

            _fileSystemMock
               .Setup(pr => pr.OpenRead(It.IsAny<string>()))
               .Returns(new MemoryStream());

            var result = await _controller.GetCardAsync(card.Id);
            var fileStreamResult = result.Should().BeOfType<FileStreamResult>().Subject;
            fileStreamResult.ContentType.Should().Be("image/png");
        }
    }
}
