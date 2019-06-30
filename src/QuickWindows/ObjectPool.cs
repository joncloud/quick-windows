using System;
using System.Collections.Generic;
using System.Linq;

namespace QuickWindows
{
    static class ObjectPool
    {
        public static ObjectPool<T> WithInitial<T>(int count)
            where T : new()
        {
            var initial = Enumerable.Range(0, count)
                .Select(_ => new T());

            return new ObjectPool<T>(initial, () => new T());
        }
    }

    class ObjectPool<T>
    {
        readonly Queue<T> _queue;
        readonly Func<T> _factory;

        public ObjectPool(Func<T> factory)
        {
            _queue = new Queue<T>();
            _factory = factory;
        }
        public ObjectPool(IEnumerable<T> initial, Func<T> factory)
        {
            _queue = new Queue<T>(initial);
            _factory = factory;
        }

        public T Rent() =>
            _queue.TryDequeue(out var item) ? item : _factory();

        public void Return(T item) =>
            _queue.Enqueue(item);
    }
}
