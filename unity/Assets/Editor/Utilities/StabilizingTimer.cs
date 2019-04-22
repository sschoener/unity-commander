using UnityEngine;

namespace Pasta.Utilities
{
    /// <summary>
    /// A stabilizing timer ticks up after it has first been interrupted. After that, each further interrupt resets the
    /// timer. The timer fires (by returning `true` from its `Update` method) either after a fixed maximum time has
    /// passed since the very first interrupt or if there haven't been any interrupts in some time. 
    /// </summary>
    public struct StabilizingTimer
    {
        public readonly float MaxTime;
        public readonly float InterruptTime;
        private float _totalTimer;
        private float _timer;

        public StabilizingTimer(float maxTime, float interruptTime)
        {
            MaxTime = maxTime;
            InterruptTime = interruptTime;
            _totalTimer = _timer = float.NegativeInfinity;
        }
		
        private bool IsActive
        {
            get { return _totalTimer >= 0;  }
        }

        public void Interrupt()
        {
            if (!IsActive)
                _totalTimer = 0;
            _timer = 0;
        }

        public bool Update()
        {
            if (!IsActive) return false;
            _totalTimer += Time.deltaTime;
            _timer += Time.deltaTime;
            if (_timer >= InterruptTime || _totalTimer >= MaxTime)
            {
                Reset();
                return true;
            }

            return false;
        }
		
        public void Reset()
        {
            _totalTimer = _timer = float.NegativeInfinity;
        }
    }
}