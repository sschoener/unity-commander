using System.Collections.Generic;

namespace Pasta.Finder
{
    public interface ISearchStepDriver<T, B>
    {
        int BatchSize { get; }
        IEnumerable<B> EnumerateNew();
        bool FilterCached(T value);
        bool FilterNew(B value);
        T MakeData(B value);
    }
}