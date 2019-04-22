namespace Pasta.Finder
{
    /// <summary>
    /// A termination handle that is always terminated.
    /// </summary>
    public class Terminated : ITerminationHandle
    {

        private Terminated()
        {
        }

        private static Terminated _instance;
        public static Terminated Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Terminated();
                return _instance;
            }
        }

        public void Terminate()
        {
        }

        public bool IsTerminating { get { return true; } }
    }
}