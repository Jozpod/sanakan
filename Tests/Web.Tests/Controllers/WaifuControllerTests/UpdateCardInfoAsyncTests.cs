using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web.Controllers;
using Sanakan.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.UpdateCardInfo(ulong, CharacterCardInfoUpdate)"/> method.
    /// </summary>
    [TestClass]
    public class UpdateCardInfoAsyncTests : Base
    {
        [TestMethod]
        public void Should_Queue_Task_And_Return_Ok()
        {
            var characterId = 1ul;
            var expected = new List<ulong>() { 0ul };
            var payload = new CharacterCardInfoUpdate
            {
                ImageUrl = "https://test.url",
                CharacterName = "new character name",
                CardSeriesTitle = "tets",
            };

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.Is<UpdateCardMessage>(pr => pr.CharacterId == characterId)))
                .Returns(true);

            var result = _controller.UpdateCardInfo(characterId, payload);
            result.Should().BeOfType<ObjectResult>();
        }
    }
}
