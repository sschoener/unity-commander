using System.Collections.Generic;

namespace Pasta.Finder
{
    /// <summary>
    /// Handles creating single search result items (the visual list item, that is) and applying data to the result item.
    /// </summary>
    /// <typeparam name="T">The data type that is searched for</typeparam>
    /// <typeparam name="V">The type that represents a result element. It must allow us to access its root visual element.</typeparam>
    public interface ISearchResultDisplayer<T, V> where V : IHaveVisualElements
    {
        /// <summary>
        /// Create a new list element for the search results.
        /// </summary>
        /// <returns></returns>
        V MakeElement();

        /// <summary>
        /// Apply the given data and search term to the specified element.
        /// </summary>
        /// <param name="searchTerm">What has been searched for?</param>
        /// <param name="element">The element to set the data on</param>
        /// <param name="data">The list of data</param>
        /// <param name="idx">The index of the particular datum in the list</param>
        void ApplyData(string searchTerm, V element, IReadOnlyList<T> data, int idx);
    }
}