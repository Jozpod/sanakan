using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            foreach (var message in Enumerable.Repeat(new OpenCardsMessage(), 100))
            {
                _blockingPriorityQueue.TryEnqueue(message);
            }
            _blockingPriorityQueue.TryEnqueue(new OpenCardsMessage()).Should().BeFalse();
        }

        [TestMethod]
        public void Should_Process_Messages()
        {
            var firstExpected = new ConnectUserMessage();
            var secondExpected = new OpenCardsMessage();
       
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(3));

            var producer = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                _blockingPriorityQueue.TryEnqueue(secondExpected);
                _blockingPriorityQueue.TryEnqueue(firstExpected);
            }, cancellationTokenSource.Token);

            var consumer = Task.Run(async () =>
            {
                var enumerable = _blockingPriorityQueue.GetEnumerable(cancellationTokenSource.Token).GetEnumerator();
                enumerable.MoveNext();
                enumerable.Current.Should().Be(firstExpected);
                enumerable.MoveNext();
                enumerable.Current.Should().Be(secondExpected);
            }, cancellationTokenSource.Token);

            Task.WhenAll(producer, consumer).Wait();
            producer.IsCompletedSuccessfully.Should().BeTrue();
            consumer.IsCompletedSuccessfully.Should().BeTrue();
        }
    }
}
