using System.Collections.Generic;

namespace Pasta.Finder
{
    /// <summary>
    /// A consumer that simply enqueues all items it receives into a queue.
    /// It is written with the assumption that one thread takes this as an `IConsumer` where as another
    /// thread is only reading the items in question.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConsumerQueue<T> : IConsumer<T>
    {
        private readonly List<T> _incoming;
        private readonly object _lock;
        private bool _isInvalidated;
        public int Count
        {
            get { return _incoming.Count; }
        }

        public ConsumerQueue()
        {
            _incoming = new List<T>();
            _lock = new object();
        }

        public void Invalidate()
        {
            if (_isInvalidated)
                return;
            lock (_lock)
            {
                _isInvalidated = true;
                _incoming.Clear();
            }
        }

        public int EmptyTo(List<T> values)
        {
            lock (_lock)
            {
                int numNew = _incoming.Count;
                values.AddRange(_incoming);
                _incoming.Clear();
                return numNew;
            }
        }
        
        void IConsumer<T>.Consume(IEnumerable<T> value)
        {
            if (_isInvalidated)
                return;
            lock (_lock)
            {
                if (_isInvalidated)
                    return;
                _incoming.AddRange(value);
            }
        }

        bool IConsumer<T>.IsValid { get { return !_isInvalidated; } }
    }
}