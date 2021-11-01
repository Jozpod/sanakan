using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue.Tests
{
    [TestClass]
    public class BlockingPriorityQueueTests
    {
        private readonly BlockingPriorityQueue _blockingPriorityQueue;

        public BlockingPriorityQueueTests()
        {
            _blockingPriorityQueue = new BlockingPriorityQueue();
        }

        [TestMethod]
        public void Should_Sort_Messages()
        {
            var firstExpected = new ConnectUserMessage();
            var secondExpected = new OpenCardsMessage();
            _blockingPriorityQueue.TryEnqueue(secondExpected);
            _blockingPriorityQueue.TryEnqueue(firstExpected);

            var enumerator = _blockingPriorityQueue.GetEnumerable().GetEnumerator();
            enumerator.MoveNext();
            enumerator.Current.Should().Be(firstExpected);
            enumerator.MoveNext();
            enumerator.Current.Should().Be(secondExpected);
        }
    }
}
