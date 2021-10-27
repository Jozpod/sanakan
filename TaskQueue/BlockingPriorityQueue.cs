using Sanakan.TaskQueue.Messages;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.TaskQueue
{
    internal class BlockingPriorityQueue : IProducerConsumerCollection<BaseMessage>
    {
        private readonly BlockingCollection<BaseMessage> _queue;

        public BlockingPriorityQueue()
        {
            _queue = new BlockingCollection<BaseMessage>();
        }

        public int Count => _queue.Count;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void CopyTo(BaseMessage[] array, int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<BaseMessage> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public BaseMessage[] ToArray()
        {
            throw new NotImplementedException();
        }

        public bool TryAdd(BaseMessage item)
        {
            _queue.Add(item);
            return true;
        }

        public bool TryTake([MaybeNullWhen(false)] out BaseMessage item)
        {
            if(_queue.TryTake(out var baseMessage))
            {
                item = baseMessage;
                return true;
            }

            item = null;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queue.GetConsumingEnumerable().GetEnumerator();
        }

        public class Test : IEnumerator
        {
            public object Current => throw new NotImplementedException();

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}
