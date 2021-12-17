using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.ShindenApi;
using Sanakan.ShindenApi.Models;
using Sanakan.TaskQueue.Messages;
using Sanakan.Web.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.Controllers.WaifuControllerTests
{
    /// <summary>
    /// Defines tests for <see cref="WaifuController.RepairCardsAsync(ulong, ulong)"/> method.
    /// </summary>
    [TestClass]
    public class RepairCardsAsyncTests : Base
    {
        [TestMethod]
        public async Task Should_Return_Ok()
        {
            var oldCharacterId = 1ul;
            var newCharacterId = 2ul;
            var expected = new List<ulong>() { 0ul };
            var characterInfoResult = new Result<CharacterInfo>
            {
                Value = new CharacterInfo
                {

                }
            };

            _shindenClientMock
                .Setup(pr => pr.GetCharacterInfoAsync(newCharacterId))
                .ReturnsAsync(characterInfoResult);

            _blockingPriorityQueueMock
                .Setup(pr => pr.TryEnqueue(It.IsAny<ReplaceCharacterIdsInCardMessage>()))
                .Returns(true);

            var result = await _controller.RepairCardsAsync(oldCharacterId, newCharacterId);
            result.Should().BeOfType<ObjectResult>();
        }
    }
}
