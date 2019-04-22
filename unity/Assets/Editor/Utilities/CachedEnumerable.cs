using System.Collections;
using System.Collections.Generic;

namespace Pasta.Utilities
{
    /// <summary>
    /// Wraps around an IEnumerable and prevents re-enumeration.
    /// When enumerated, it caches the results of the wrapped enumerable and only queries it for
    /// more items if more items are requested than cached.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CachedEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> _iter;
        private readonly List<T> _data;
        private int _currentIndex;

        public CachedEnumerable(IEnumerable<T> enumerable)
        {
            _iter = enumerable.GetEnumerator();
            _data = new List<T>();
            _currentIndex = -1;
        }

        /// <summary>
        /// Is the underlying enumerable completely in cache?
        /// </summary>
        /// <value></value>
        public bool IsAtEnd { get; private set; }
        public int CachedCount { get { return _data.Count; } }

        /// <summary>
        /// Fetches data from the underlying enumerable until the given number of items are in the cache.
        /// </summary>
        /// <param name="upTo"></param>
        public void FetchData(int upTo)
        {
            while (_currentIndex < upTo && _iter.MoveNext())
            {
                _data.Add(_iter.Current);
                _currentIndex++;
            }

            if (_currentIndex < upTo)
                IsAtEnd = true;
        }

        public T this[int idx]
        {
            get
            {
                FetchData(idx);
                return _data[idx];
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; ; i++)
            {
                FetchData(i);
                if (i < CachedCount)
                    yield return _data[i];
                else
                    yield break;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}