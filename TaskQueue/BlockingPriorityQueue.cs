using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sanakan.TaskQueue
{
    internal class BlockingPriorityQueue : IBlockingPriorityQueue, IDisposable
    {
        private readonly BaseMessage[] _items;
        private readonly object _syncRoot = new();
        private readonly SemaphoreSlim _semaphoreSlim;
        private int _head;

        public BlockingPriorityQueue()
        {
            _items = new BaseMessage[100];
            _head = -1;
            _semaphoreSlim = new(0, 1);
        }

        public bool TryEnqueue(BaseMessage message)
        {
            lock (_syncRoot)
            {
                if(_head == _items.Length - 1)
                {
                    return false;
                }

                Interlocked.Increment(ref _head);
                _items[_head] = message;

                if (_head == 0)
                {
                    if (_semaphoreSlim.CurrentCount == 0)
                    {
                        _semaphoreSlim.Release();
                        return true;
                    }
                }
            }

            return true;
        }

        public async IAsyncEnumerable<BaseMessage> GetAsyncEnumerable([EnumeratorCancellation] CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                while (_head == -1)
                {
                    await _semaphoreSlim.WaitAsync(token);
                }

                BaseMessage item;
                lock (_syncRoot)
                {
                    item = _items[_head];
                    _items[_head] = null;
                    Interlocked.Decrement(ref _head);
                }

                yield return item;
            }

            token.ThrowIfCancellationRequested();
        }

        public void Dispose()
        {
            _semaphoreSlim.Dispose();
        }
    }
}
