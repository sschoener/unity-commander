using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pasta.Finder
{
    public class IncrementalSearch<T, S, F> : IProducer<T> where S : IProducer<T>
    {
        private readonly Stack<S> _steps;
        private readonly IIncrementalSearchDriver<S, F> _driver;

        public IncrementalSearch(IIncrementalSearchDriver<S, F> driver)
        {
            _steps = new Stack<S>();
            _driver = driver;
        }

        public IEnumerable Produce(IConsumer<T> consumer, ITimeBudget budget)
        {
            if (_steps.Count == 0)
                return Enumerable.Empty<int>();
            return _steps.Peek().Produce(consumer, budget);
        }

        public void Clear()
        {
            _steps.Clear();
        }

        private ComparisonResult Compare(F filter, S step)
        {
            return _driver.CompareSpecificity(filter, _driver.GetFilter(step));
        }
        
        public void SetMatcher(F filter)
        {
            while (_steps.Count > 0 && Compare(filter, _steps.Peek()) != ComparisonResult.Greater)
                _steps.Pop();
            if (_steps.Count == 0)
                _steps.Push(_driver.GetRoot(filter));
            else if (Compare(filter, _steps.Peek()) != ComparisonResult.Equal)
            {
                var parent = _steps.Peek();
                _steps.Push(_driver.GetSub(filter, parent));
            }
        }
    }
}