namespace Pasta.Finder
{
    public interface IIncrementalSearchDriver<S, F>
    {
        ComparisonResult CompareSpecificity(F left, F right);
        F GetFilter(S searcher);
        S GetRoot(F filter);
        S GetSub(F filter, S parent);
    }
}