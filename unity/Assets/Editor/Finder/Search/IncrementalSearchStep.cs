using System.Collections;
using System.Collections.Generic;

namespace Pasta.Finder
{
    public class IncrementalSearchStep<TData, TBaseData, TDriver> : IProducer<TData> where TDriver : ISearchStepDriver<TData, TBaseData>
    {
        private readonly IncrementalSearchStep<TData, TBaseData, TDriver> _parent;
        public readonly TDriver Driver;
        private List<TData> _cachedResults;

        public IncrementalSearchStep(TDriver driver, IncrementalSearchStep<TData, TBaseData, TDriver> parent)
        {
            Driver = driver;
            _parent = parent;
        }

        private IEnumerable PostProcess(IEnumerable steps, IConsumer<TData> consumer, List<TData> buf)
        {
            foreach (var s in steps)
            {
                PostProcess(consumer, buf);
                yield return null;
            }

            PostProcess(consumer, buf);
        }

        private void PostProcess(IConsumer<TData> consumer, List<TData> buf)
        {
            _cachedResults.AddRange(buf);
            consumer.Consume(buf);
            buf.Clear();
        }

        private IEnumerable ConsumeCache(IConsumer<TData> consumer, List<TData> buffer, ITimeBudget budget)
        {
            if (_cachedResults == null)
            {
                _cachedResults = new List<TData>();
                if (_parent != null)
                {
                    int n = Driver.BatchSize;
                    var steps = budget.Take(_parent._cachedResults, buffer, n, Driver.FilterCached);
                    foreach (var s in PostProcess(steps, consumer, buffer))
                        yield return s;
                }
            }
            else
                consumer.Consume(_cachedResults);
        }

        private IEnumerable ProduceNew(IConsumer<TData> consumer, List<TData> buffer, ITimeBudget budget)
        {
            int n = Driver.BatchSize;
            var newSteps = budget.Take(Driver.EnumerateNew(), buffer, n,
                Driver.FilterNew, Driver.MakeData
            );
            foreach (var s in PostProcess(newSteps, consumer, buffer))
                yield return s;
        }

        public IEnumerable Produce(IConsumer<TData> consumer, ITimeBudget budget)
        {
            var buffer = new List<TData>();
            foreach (var s in ConsumeCache(consumer, buffer, budget))
                yield return s;
            foreach (var s in ProduceNew(consumer, buffer, budget))
                yield return s;
        }
    }
}