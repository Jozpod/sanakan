using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.Game.Services;
using Sanakan.Game.Services.Abstractions;
using System;

namespace Sanakan.Game.Tests
{
    /// <summary>
    /// Defines tests for <see cref="ISlotMachine"/> class.
    /// </summary>
    [TestClass]
    public class SlotMachineTests
    {
        protected readonly ISlotMachine _slotMachine;
        protected readonly Mock<IRandomNumberGenerator> _randomNumberGeneratorMock = new(MockBehavior.Strict);

        public SlotMachineTests()
        {
            _slotMachine = new SlotMachine(_randomNumberGeneratorMock.Object);
        }

        [TestMethod]
        public void Should_Draw_And_Return_Text()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);

            var expected = @"✖ 🐷🐷🐷🐷🐷
✔ 🐷🐷🐷🐷🐷
✖ 🐷🐷🐷🐷🐷".Replace("\r", "");
            var actual = _slotMachine.Draw(user);
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void Should_Return_Amount_To_Pay()
        {
            var smConfig = new SlotMachineConfig
            {
                Beat = SlotMachineBeat.b1,
                Multiplier = SlotMachineBeatMultiplier.x2,
                Rows = SlotMachineSelectedRows.r3,
            };

            var expected = 6;
            var actual = _slotMachine.ToPay(smConfig);
            actual.Should().Be(expected);
        }

        [TestMethod]
        public void Should_Play_Slot_Machine()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(0, 1000))
                .Returns(1);

            var expected = 20;
            var actual = _slotMachine.Play(user);
            actual.Should().Be(expected);
        }
    }
}
