using Sanakan.TaskQueue.Messages;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sanakan.TaskQueue
{
    internal class BlockingPriorityQueue : IBlockingPriorityQueue, IDisposable
    {
        private readonly BaseMessage[] _items;
        private readonly object _syncRoot = new();
        private readonly ManualResetEventSlim _manualResetEventSlim;
        private readonly SemaphoreSlim _semaphoreSlim;
        private int _head;

        public BlockingPriorityQueue()
        {
            _items = new BaseMessage[100];
            _head = -1;
            _manualResetEventSlim = new ();
            _semaphoreSlim = new(1);
        }

        public bool TryEnqueue(BaseMessage message)
        {
            lock (_syncRoot)
            {
                if(_head + 1 == _items.Length)
                {
                    return false;
                }
                _head++;
                _items[_head] = message;
            }

            if(_semaphoreSlim.CurrentCount == 0)
            {
                _semaphoreSlim.Release();
            }

            if (!_manualResetEventSlim.IsSet)
            {
                _manualResetEventSlim.Set();
            }

            return true;
        }

        public async IAsyncEnumerable<BaseMessage> GetAsyncEnumerable([EnumeratorCancellation] CancellationToken token = default)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                if (_head == -1)
                {
                    await _semaphoreSlim.WaitAsync(token);
                }

                BaseMessage item;
                lock (_syncRoot)
                {
                    item = _items[_head];
                    _items[_head] = null;
                    _head--;
                }

                yield return item;
            }
        }

        public IEnumerable<BaseMessage> GetEnumerable(CancellationToken token = default)
        {
            while(true)
            {
                token.ThrowIfCancellationRequested();

                if(_head == -1)
                {
                    _manualResetEventSlim.Reset();
                    _manualResetEventSlim.Wait(token);
                }

                BaseMessage item;
                lock (_syncRoot)
                {
                    item = _items[_head];
                    _items[_head] = null;
                    _head--;
                }

                yield return item;
            }
        }

        public void Dispose()
        {
            _manualResetEventSlim.Dispose();
        }
    }
}
