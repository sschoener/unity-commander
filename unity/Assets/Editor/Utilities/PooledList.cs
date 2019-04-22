using System.Collections;
using System.Collections.Generic;
using Pasta.Utilities;

namespace Pasta.Utilities
{
    /// <summary>
    /// Represents a list with a number of always-allocated elements that
    /// are returned to an underlying lifetime manager when the list shrinks.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectList<T> : IEnumerable<T>
    {
        private readonly ILifetimeManager<T> _pool;
        private readonly List<T> _entries;

        public ObjectList(ILifetimeManager<T> pool)
        {
            _pool = pool;
            _entries = new List<T>();
        }
        
        public int Count
        {
            get { return _entries.Count;  }
        }

        public void SetSize(int size)
        {
            int tooMany = _entries.Count - size;
            if (tooMany > 0)
            {
                int delete = _entries.Count - tooMany;
                for (int i = _entries.Count - 1; i >= delete; i--)
                    _pool.Release(_entries[i]);
                _entries.RemoveRange(delete, tooMany);
            }

            int tooFew = size - _entries.Count;
            if (tooFew > 0)
            {
                for (int i = 0; i < tooFew; i++)
                    _entries.Add(_pool.Instantiate());
            }
        }

        public void Clear()
        {
            SetSize(0);
        }

        public T AddOne()
        {
            var entry = _pool.Instantiate();
            _entries.Add(entry);
            return entry;
        }

        public T this[int idx]
        {
            get { return _entries[idx];  }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}