using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pasta.Finder
{
    public class SceneTransformIterator : IEnumerator<Transform>
    {
        private readonly List<Transform> _roots;
        private int _rootIndex;
        private Transform _current;
        private int _depth;

        public SceneTransformIterator()
        {
            _roots = SceneWalker.SceneRoots().Select(g => g.transform).ToList();
            _rootIndex = -1;
        }

        public SceneTransformIterator(SceneTransformIterator other)
        {
            _roots = other._roots;
            _rootIndex = other._rootIndex;
            _current = other._current;
            _depth = other._depth;
        }
        
        public bool MoveNext()
        {
            if (_rootIndex >= 0)
            {
                if (_current.childCount > 0)
                {
                    _current = _current.GetChild(0);
                    _depth++;
                    return true;
                }
    
                while(_depth > 0)
                {
                    int idx = _current.GetSiblingIndex() + 1;
                    _current = _current.parent.GetChild(idx);
                    _depth--;
                }
            }
            _rootIndex += 1;
            if (_rootIndex < _roots.Count)
            {
                _current = _roots[_rootIndex];
                return true;
            }
            return false;
        }

        public void Reset() { }

        public Transform Current
        {
            get { return _current; }
        }

        object IEnumerator.Current => Current;

        public void Dispose() { }
    }
}