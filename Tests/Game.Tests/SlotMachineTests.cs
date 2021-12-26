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
        [DataRow(SlotMachineSelectedRows.r1, 1)]
        [DataRow(SlotMachineSelectedRows.r2, 1)]
        [DataRow(SlotMachineSelectedRows.r3, 1)]
        [DataRow(SlotMachineSelectedRows.r1, 0)]
        [DataRow(SlotMachineSelectedRows.r2, 0)]
        [DataRow(SlotMachineSelectedRows.r3, 0)]
        public void Should_Draw_And_Return_Text(SlotMachineSelectedRows selectedRows, int psayMode)
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.SMConfig = new SlotMachineConfig
            {
                Rows = selectedRows,
                PsayMode = psayMode,
            };

            var actual = _slotMachine.Draw(user);
            actual.Should().NotBeNullOrEmpty();
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
        [DataRow(SlotMachineSelectedRows.r1, 1, 80)]
        [DataRow(SlotMachineSelectedRows.r2, 1, 160)]
        [DataRow(SlotMachineSelectedRows.r3, 1, 240)]
        [DataRow(SlotMachineSelectedRows.r1, 0, 40)]
        [DataRow(SlotMachineSelectedRows.r2, 0, 80)]
        [DataRow(SlotMachineSelectedRows.r3, 0, 120)]
        public void Should_Play_Slot_Machine(SlotMachineSelectedRows selectedRows, int psayMode, int expected)
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);
            user.SMConfig = new SlotMachineConfig
            {
                Beat = SlotMachineBeat.b1,
                Multiplier = SlotMachineBeatMultiplier.x2,
                Rows = selectedRows,
                PsayMode = psayMode,
            };

            _randomNumberGeneratorMock
                .Setup(pr => pr.GetRandomValue(0, 1000))
                .Returns(1);

            var actual = _slotMachine.Play(user);
            actual.Should().Be(expected);
        }
    }
}
