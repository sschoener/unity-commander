using System;
using System.Collections;
using System.Collections.Generic;

namespace Pasta.Finder
{
    public static class TimeBudgetExtensions
    {
        public static IEnumerable Repeat(this ITimeBudget budget, Func<bool> f)
        {
            while (true)
            {
                while (budget.MillisRemaining > 0)
                {
                    if (!f())
                        yield break;
                }

                yield return null;
            }
        }

        public static IEnumerable Take<T>(this ITimeBudget budget,
            IEnumerable<T> ts,
            ICollection<T> results,
            int steps,
            Predicate<T> where
        )
        {
            return budget.Take(ts, results, steps, where, d => d);
        }

        public static IEnumerable Take<T, R>(this ITimeBudget budget,
            IEnumerable<T> ts,
            ICollection<R> results,
            int steps,
            Predicate<T> where,
            Func<T, R> selector
        )
        {
            var iter = ts.GetEnumerator();
            return budget.Repeat(() =>
                {
                    bool hasNext = true;
                    for (int i = 0; i < steps && (hasNext = iter.MoveNext()); i++)
                    {
                        if (where(iter.Current))
                            results.Add(selector(iter.Current));
                    }

                    if (!hasNext)
                        iter.Dispose();
                    return hasNext;
                }
            );
        }
    }
}