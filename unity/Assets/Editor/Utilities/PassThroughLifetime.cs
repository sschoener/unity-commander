namespace Pasta.Utilities
{
    public class PassThroughLifetime<T> : ILifetimeManager<T>
    {
        private readonly ILifetimeManager<T> _parent;
        private readonly System.Action<T> _onInstantiate;
        private readonly System.Action<T> _onRelease;

        public PassThroughLifetime(
            ILifetimeManager<T> parent,
            System.Action<T> onInstantiate,
            System.Action<T> onRelease
        )
        {
            _parent = parent;
            _onInstantiate = onInstantiate;
            _onRelease = onRelease;
        }

        public void Release(T value)
        {
            if (_onRelease != null)
                _onRelease(value);
            _parent.Release(value);
        }

        public T Instantiate()
        {
            var value = _parent.Instantiate();
            if (_onInstantiate != null)
                _onInstantiate(value);
            return value;
        }
    }
}