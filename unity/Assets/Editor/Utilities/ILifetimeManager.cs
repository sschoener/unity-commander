namespace Pasta.Utilities
{
    public interface ILifetimeManager<T>
    {
        void Release(T value);
        T Instantiate();
    }
}