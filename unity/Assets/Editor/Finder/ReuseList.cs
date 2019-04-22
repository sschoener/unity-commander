using System.Collections.Generic;
using Pasta.Utilities;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleSheets;

namespace Pasta.Finder
{
    public class ReuseList<T> where T : IHaveVisualElements
    {
        private readonly ILifetimeManager<T> _pool;
        private readonly List<T> _entries;
        private readonly VisualContainer _parent;
        public VisualContainer Container
        {
            get { return _parent; }
        }
		
        public ReuseList(System.Func<T> spawner)
        {
            _parent = new VisualContainer();
            _entries = new List<T>();
            _pool = ListPool.New(() =>
                {
                    var entry = spawner();
                    _parent.Add(entry.Element);
                    return entry;
                }
            );
        }
        
        public int Size
        {
            get { return _entries.Count; }
        }

        public void SetSize(int size)
        {
            int tooMany = _entries.Count - size;
            if (tooMany > 0)
            {
                int delete = _entries.Count - tooMany;
                for (int i = _entries.Count - 1; i >= delete; i--)
                {
                    _entries[i].Element.CheapDisable();
                    _pool.Release(_entries[i]);
                }
                _entries.RemoveRange(delete, tooMany);
            }

            int tooFew = size - _entries.Count;
            if (tooFew > 0)
            {
                for (int i = 0; i < tooFew; i++)
                {
                    var entry = _pool.Instantiate();
                    entry.Element.CheapEnable();
                    entry.Element.BringToFront();
                    _entries.Add(entry);
                }
            }
        }

        public T this[int idx]
        {
            get { return _entries[idx];  }
        }

        public void CycleForward(int amount)
        {
            if (_entries.Count == 0)
                return;
            amount %= _entries.Count;
            var tmp = _entries.ToArray();
            for (int i = 0; i < _entries.Count; i++)
            {
                _entries[i] = tmp[(i - amount + _entries.Count) % _entries.Count];
                _entries[i].Element.BringToFront();
            }
            
        }
    }
}