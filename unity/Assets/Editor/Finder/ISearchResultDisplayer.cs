using System.Collections.Generic;

namespace Pasta.Finder
{
    public interface ISearchResultDisplayer<T, V> where V : IHaveVisualElements
    {
        V MakeElement();
        void ApplyData(string searchTerm, V element, IReadOnlyList<T> data, int idx);
    }
}