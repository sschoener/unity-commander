using System.Collections.Generic;
using UnityEngine;

namespace Pasta.Finder
{
    public interface ISearchResultProcessor<T>
    {
        void OnSubmit(IReadOnlyList<T> results, int selection, EventModifiers modifiers);
        void OnSelect(IReadOnlyList<T> results, int selection);
    }
}