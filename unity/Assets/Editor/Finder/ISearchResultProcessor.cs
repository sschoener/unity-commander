using System.Collections.Generic;
using UnityEngine;

namespace Pasta.Finder
{
    /// <summary>
    /// Handle the interaction with search results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISearchResultProcessor<T>
    {
        /// <summary>
        /// Do whatever action is necessary when the user submits a search result item by pressing Enter.
        /// </summary>
        /// <param name="results">The list of search results.</param>
        /// <param name="selection">The selected index in the search results.</param>
        /// <param name="modifiers">Any key modifiers that were active when the user submitted</param>
        void OnSubmit(IReadOnlyList<T> results, int selection, EventModifiers modifiers);

        /// <summary>
        /// Do something when the user selects an item (e.g. highlight the search result somewhere else).
        /// </summary>
        /// <param name="results">The list fo search results.</param>
        /// <param name="selection">The selected index in the search results.</param>
        void OnSelect(IReadOnlyList<T> results, int selection);
    }
}