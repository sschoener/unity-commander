using System;
using System.Collections.Generic;

namespace Pasta.Utilities
{
    public static class ListPool
    {
        public static ListPool<T> New<T>(Func<T> spawner)
        {
            return new ListPool<T>(new Spawner<T>(spawner));
        }
    }
    
    public class ListPool<T> : ILifetimeManager<T>
    {
        private readonly Stack<T> _elements;
        private readonly ILifetimeManager<T> _parent;

        public ListPool(ILifetimeManager<T> parent)
        {
            _parent = parent;
            _elements = new Stack<T>();
        }

        public void ClearPool()
        {
            while (_elements.Count > 0)
                _parent.Release(_elements.Pop());
        }

        public void Release(T t)
        {
            _elements.Push(t);
        }

        public T Instantiate()
        {
            if (_elements.Count > 0)
                return _elements.Pop();
            return _parent.Instantiate();
        }
    }
}