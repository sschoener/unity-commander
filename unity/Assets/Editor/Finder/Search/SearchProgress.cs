namespace Pasta.Finder
{
    public struct SearchProgress
    {
        public readonly int NumResults;
        public readonly SearchState State;
        public bool NoSearch { get { return State == SearchState.NotStarted && NumResults == 0; } }
        public bool InProgress { get { return State == SearchState.Searching; } }
        
        public SearchProgress(int numResults, SearchState state)
        {
            NumResults = numResults;
            State = state;
        }

        public bool Equals(SearchProgress progress)
        {
            return NumResults == progress.NumResults && State == progress.State;
        }
    }
}