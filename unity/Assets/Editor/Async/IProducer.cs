using System.Collections;

namespace Pasta.Finder
{
    public interface IProducer<T>
    {
        IEnumerable Produce(IConsumer<T> consumer, ITimeBudget budget);
    }
}