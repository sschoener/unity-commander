namespace Pasta.Finder
{
    /// <summary>
    /// Represents the position of a cursor in a list of search results.
    /// </summary>
    public interface ISelectionPosition
    {
        /// <summary>
        /// The index of the cursor in the list.
        /// </summary>
        /// <value></value>
        int Index { get; }

        /// <summary>
        /// The number of visible items at once.
        /// </summary>
        /// <value></value>
        int WindowSize { get; }

        /// <summary>
        /// The total number of results up until now.
        /// </summary>
        /// <value></value>
        int Count { get; }
    }
}