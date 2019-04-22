namespace Pasta.Utilities
{
    public class Spawner<T> : ILifetimeManager<T>
    {
        private readonly System.Func<T> _createNew;
        public Spawner(System.Func<T> createNew)
        {
            _createNew = createNew;
        }

        public T Instantiate()
        {
            return _createNew();
        }

        public void Release(T value)
        {
            // do nothing!
        }
    }
}