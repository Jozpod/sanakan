﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Services.Abstractions;
using System;

namespace Sanakan.Game.Tests.WaifuServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IWaifuService.DeleteCardImageIfExist(Card)"/> method.
    /// </summary>
    [TestClass]
    public class DeleteCardImageIfExistTests : Base
    {
        [TestMethod]
        public void Should_Delete_Files()
        {
            var card = new Card(1, "test", "test", 100, 50, Rarity.SSS, Dere.Bodere, DateTime.Now);

            _fileSystemMock
                .Setup(pr => pr.Exists(It.IsAny<string>()))
                .Returns(true);

            _fileSystemMock
                .Setup(pr => pr.Delete(It.IsAny<string>()));

            _waifuService.DeleteCardImageIfExist(card);
        }
    }
}
