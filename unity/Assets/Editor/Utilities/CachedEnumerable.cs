using System.Collections;
using System.Collections.Generic;

namespace Pasta.Utilities
{
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
        
        public bool IsAtEnd { get; private set; }
        public int CachedCount
        {
            get { return _data.Count; }
        }

        public void GetData(int upTo)
        {
            while (_currentIndex < upTo && _iter.MoveNext())
            {
                _data.Add(_iter.Current);
                _currentIndex++;
            }

            if (_currentIndex < upTo)
                IsAtEnd = true;
        }

        public void Force()
        {
            while (!IsAtEnd)
                GetData(2 * CachedCount + 1);
        }

        public T this[int idx]
        {
            get
            {
                GetData(idx);
                return _data[idx];
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; ; i++)
            {    
                GetData(i);
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