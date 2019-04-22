namespace Pasta.Finder
{
    /// <summary>
    /// Represents the position of a cursor in a list of search results.
    /// </summary>
    public interface ISelectionPosition
    {
        int Index { get; }
        int WindowSize { get; }
        int Count { get; }
    }
}