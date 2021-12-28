using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.TaskQueue.Messages;
using System;
using System.Linq;
using System.Threading;
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
        public void Should_Limit_Queue()
        {
            foreach (var message in Enumerable.Repeat(new GiveCardsMessage(), 100))
            {
                _blockingPriorityQueue.TryEnqueue(message);
            }
            _blockingPriorityQueue.TryEnqueue(new GiveCardsMessage()).Should().BeFalse();
        }

        [TestMethod]
        public async Task Should_Block_Queue()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(1));
            var enumerable = _blockingPriorityQueue.GetAsyncEnumerable(cancellationTokenSource.Token).GetAsyncEnumerator();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () => {
                await enumerable.MoveNextAsync();
            });
        }

        [TestMethod]
        public async Task Should_Return_Message()
        {
            var firstExpected = new ConnectUserMessage();

            var cancellationTokenSource = new CancellationTokenSource();

            _blockingPriorityQueue.TryEnqueue(firstExpected);
            var enumerable = _blockingPriorityQueue.GetAsyncEnumerable(cancellationTokenSource.Token).GetAsyncEnumerator();
            await  enumerable.MoveNextAsync();
            enumerable.Current.Should().Be(firstExpected);
        }

        [TestMethod]
        public void Should_Sort_Messages_By_Priority()
        {
            var firstExpected = new ConnectUserMessage();
            var secondExpected = new GiveCardsMessage();

            var cancellationTokenSource = new CancellationTokenSource();

            _blockingPriorityQueue.TryEnqueue(secondExpected);
            _blockingPriorityQueue.TryEnqueue(firstExpected);

            var enumerable = _blockingPriorityQueue.GetAsyncEnumerable(cancellationTokenSource.Token).GetAsyncEnumerator();
            enumerable.MoveNextAsync();
            var actual = enumerable.Current;
            actual.Should().Be(firstExpected);
            enumerable.MoveNextAsync();
            actual = enumerable.Current;
            actual.Should().Be(secondExpected);
        }
    }
}
