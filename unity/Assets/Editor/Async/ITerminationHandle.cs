namespace Pasta.Finder
{
    /// <summary>
    /// Represents an ongoing process that can be terminated, like a search.
    /// </summary>
    public interface ITerminationHandle
    {
        /// <summary>
        /// Kindly ask for termination.
        /// </summary>
        void Terminate();
        
        /// <summary>
        /// Whether the process is terminating or terminated (whether by itself or by someone else doesn't matter). 
        /// </summary>
        bool IsTerminating { get; }
    }
}