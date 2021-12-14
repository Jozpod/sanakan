﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using System;

namespace Sanakan.Game.Tests.EventServiceTests
{
    /// <summary>
    /// Defines tests for <see cref="IEventsService.ExecuteEvent(EventType, User, Card, string)"/> method.
    /// </summary>
    [TestClass]
    public class ExecuteEventTests : Base
    {
        [TestMethod]
        [DataRow(EventType.ChangeDere, true)]
        [DataRow(EventType.AddReset, true)]
        [DataRow(EventType.DecDef, true)]
        [DataRow(EventType.MoreExp, true)]
        [DataRow(EventType.DecAtk, true)]
        public void Should_Execute_Event(EventType eventType, bool boolValue)
        {
            var user = new User(1ul, DateTime.UtcNow);
            var card = new Card(1ul, "title", "name", 100, 50, Rarity.B, Dere.Bodere, DateTime.UtcNow);
            var message = "test message";

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(1);

            var result = _eventsService.ExecuteEvent(eventType, user, card, message);
            result.Item1.Should().Be(boolValue);
            result.Item2.Should().Be(message);
        }
    }
}
