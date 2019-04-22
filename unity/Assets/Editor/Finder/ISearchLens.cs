namespace Pasta.Finder
{
    public interface ISearchLens<T>
    {
        ITerminationHandle SetSearchData(ISelectionPosition selectionPosition, IConsumer<T> consumer, string search);
    }
}