using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SharpDX.Core
{
    class ThreadDictionary<T>
    {
        private readonly ConcurrentDictionary<Thread, T> _items;
        private readonly Func<T> _createEvent;


        public ThreadDictionary(Func<T> createEvent) {
            this._createEvent = createEvent;

            _items = new ConcurrentDictionary<Thread, T>();
        }

        public T Get() {
            var key = Thread.CurrentThread;

            T item;
            if (_items.TryGetValue(key, out item)) return item;

            item = _createEvent();
            _items[key] = item;
            return item;
        }
    }
}
