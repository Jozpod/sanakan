using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.Game.Services;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void Should_Play_Slot_Machine()
        {
            var utcNow = DateTime.UtcNow;
            var user = new User(1ul, utcNow);

            var win = _slotMachine.Play(user);
        }
    }
}
