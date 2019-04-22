using System.Diagnostics;

namespace Pasta.Finder
{
    /// <summary>
    /// Keeps track of a time-budget in milli-seconds.
    /// </summary>
    public class TimeBudget : ITimeBudget
    {
        private readonly int _millis;
        private readonly Stopwatch _watch;
        
        public TimeBudget(int millis)
        {
            _millis = millis;
            _watch = new Stopwatch();
            _watch.Start();
        }

        public void Reset()
        {
            _watch.Restart();
        }
        
        int ITimeBudget.MillisRemaining
        {
            get
            {
                var r = _millis - _watch.ElapsedMilliseconds;
                if (r < 0)
                    r = 0;
                return  (int) r;
            }
        }
    }
}