using System.Collections;
using UnityEditor;

namespace Pasta.Finder
{
    public class SearchProcess<T> : ITerminationHandle
    {
        private bool _isTerminating;
        private readonly ISelectionPosition _position;
        private readonly TimeBudget _budget;
        private readonly IEnumerator _producer;

        public SearchProcess(IProducer<T> producer, ISelectionPosition position, IConsumer<T> consumer)
        {
            _position = position;
            _budget = new TimeBudget(5);
            _producer = producer.Produce(consumer, _budget).GetEnumerator();
            EditorApplication.update += Update;
            Update();
        }

        private void Update()
        {
            if (_position.Index + 5 * _position.WindowSize < _position.Count)
                return;
            _budget.Reset();
            if (!_producer.MoveNext())
                (this as ITerminationHandle).Terminate();
        }

        void ITerminationHandle.Terminate()
        {
            _isTerminating = true;
            EditorApplication.update -= Update;
        }

        bool ITerminationHandle.IsTerminating
        {
            get { return _isTerminating; }
        }
    }
}