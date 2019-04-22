using System;
using System.Collections.Generic;
using Pasta.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Pasta.Finder
{
    public class Finder<T, V> where V : IHaveVisualElements
    {
        private readonly VisualElement _searchField;
        private readonly List<T> _resultData;
        private readonly SelectionPosition _selectionPosition;
        private readonly ReuseSelectionList<V> _resultDisplay;
        private ConsumerQueue<T> _incomingData;
        private ITerminationHandle _terminationHandle;

        private readonly ISearchResultProcessor<T> _processor;
        private readonly ISearchResultDisplayer<T, V> _displayer;
        private readonly ISearchLens<T> _searchLens;

        private StabilizingTimer _searchTriggerTimer = new StabilizingTimer(0f, 0f);

        private string _currentSearchTerm;
        private string _nextSearchTerm;
        private SearchProgress _searchProgress;

        public event Action<SearchProgress> SearchStateChanged;

        /// <summary>
        /// The number of results that will be visible at once.
        /// </summary>
        public int NumVisibleResults { get; private set; }

        /// <summary>
        /// The index of the selected element within the visible results. Setting this to 0 will try to make the
        /// selection the first entry, always. Use negative values to index from the back.
        /// </summary>
        public int IndexOfSelection { get; private set; }

        /// <summary>
        /// The time in seconds without keyboard inputs that is needed before a search is triggered.
        /// </summary>
        public float InterruptTime
        {
            get { return _searchTriggerTimer.InterruptTime; }
            set { _searchTriggerTimer = new StabilizingTimer(_searchTriggerTimer.MaxTime, value); }
        }

        /// <summary>
        /// The maximum time in seconds that is allowed to pass between a keypress and a search update.
        /// </summary>
        public float MaxTimeToUpdate
        {
            get { return _searchTriggerTimer.MaxTime; }
            set { _searchTriggerTimer = new StabilizingTimer(value, _searchTriggerTimer.InterruptTime); }
        }
        
        public VisualContainer Container
        {
            get { return _resultDisplay.Container; }
        }
		
        public Finder(
            TextField searchField,
            ISearchLens<T> searchLens,
            ISearchResultProcessor<T> processor,
            ISearchResultDisplayer<T, V> displayer,
            int visibleResults=10,
            int indexOfSelection=0
        )
        {
            _searchLens = searchLens;
            _processor = processor;
            _displayer = displayer;
            _resultData = new List<T>();
            _selectionPosition = new SelectionPosition(this);
            NumVisibleResults = visibleResults;
            if (indexOfSelection >= NumVisibleResults)
                throw new ArgumentException("The index of the selection must be less than the number of results.");
            IndexOfSelection = indexOfSelection;
			
            searchField.RegisterCallback<KeyDownEvent>(HandleSearchFieldInputs);
            searchField.OnValueChanged(e => UpdateSearch(e.newValue));
			
            _resultDisplay = new ReuseSelectionList<V>(
                _displayer.MakeElement,
                (e, i) => _displayer.ApplyData(_currentSearchTerm, e, _resultData, i),
                new DataSource(_resultData), 
                NumVisibleResults,
                IndexOfSelection
            );
            _resultDisplay.Container.CheapDisable();
            _resultDisplay.Prewarm();
            EditorApplication.update += Update;
        }

        private void Update()
        {
            if (_searchTriggerTimer.Update())
                UpdateSearch();
            var newSearchState = GetSearchState();
            if (!_searchProgress.Equals(newSearchState))
            {
                _searchProgress = newSearchState;
                if (SearchStateChanged != null)
                    SearchStateChanged(_searchProgress);
            }
                
            if (_incomingData == null)
                return;

            int numNew = _incomingData.EmptyTo(_resultData);
            if (numNew > 0)
                _resultDisplay.CheckForNew();
        }

        private SearchProgress GetSearchState()
        {
            var state = SearchState.Searching;
            if (_terminationHandle == null)
                state = SearchState.NotStarted;
            else if (_terminationHandle.IsTerminating)
                state = SearchState.Done;
            return new SearchProgress(_resultData.Count, state);
        }

        public void Stop()
        {
            EditorApplication.update -= Update;
        }
		
        private int Max(int a, int b)
        {
            return a > b ? a : b;
        }
		
        private void HandleSearchFieldInputs(KeyDownEvent e)
        {
            switch (e.keyCode)
            {
                case KeyCode.DownArrow:
                    Scroll(1);
                    break;
                case KeyCode.UpArrow:
                    Scroll(-1);
                    break;
                case KeyCode.PageDown:
                    Scroll(Max(1, NumVisibleResults - 1));
                    break;
                case KeyCode.PageUp:
                    Scroll(-Max(1, NumVisibleResults - 1));
                    break;
                case KeyCode.Return:
                    Submit(e.modifiers);
                    break;
                default:
                    return;
            }
            e.PreventDefault();
            e.StopPropagation();
        }

        private void Scroll(int amount)
        {
            if (amount == 0)
                return;
            _resultDisplay.Scroll(amount);
            if (IsSelectionValid)
                _processor.OnSelect(_resultData, _resultDisplay.SelectionIndex);
        }

        private void Submit(EventModifiers modifiers)
        {
            if (IsSelectionValid)
                _processor.OnSubmit(_resultData, _resultDisplay.SelectionIndex, modifiers);
        }

        private bool IsSelectionValid
        {
            get { return _resultData != null && _resultData.Count > _resultDisplay.SelectionIndex && _resultDisplay.SelectionIndex >= 0; }
        }

        private void UpdateSearch(string newSearch)
        {
            _nextSearchTerm = newSearch;
            if (_searchTriggerTimer.InterruptTime > 0)
                _searchTriggerTimer.Interrupt();
            else
                UpdateSearch();
        }

        private void UpdateSearch()
        {
            if (_terminationHandle != null)
            {
                if (!_terminationHandle.IsTerminating)
                    _terminationHandle.Terminate();
                _incomingData.Invalidate();
            }

            _currentSearchTerm = _nextSearchTerm;
            _incomingData = new ConsumerQueue<T>();
            _resultData.Clear();
            _terminationHandle = _searchLens.SetSearchData(_selectionPosition, _incomingData, _currentSearchTerm);
            // if the search process is synchronous, we can already take all of the results now. Otherwise, this is a
            // no-op.
            bool hasNew = _incomingData.EmptyTo(_resultData) > 0;
            _resultDisplay.JumpTo(0);
            if (hasNew)
                _resultDisplay.CheckForNew();

            if (IsSelectionValid)
                _processor.OnSelect(_resultData, _resultDisplay.SelectionIndex);
            else
                _processor.OnSelect(_resultData, -1); 
        }

        private class SelectionPosition : ISelectionPosition
        {
            private readonly Finder<T, V> _finder;
            public SelectionPosition(Finder<T, V> finder)
            {
                _finder = finder;
            }

            public int Index { get { return _finder._resultDisplay.SelectionIndex; } }
            public int WindowSize { get { return _finder._resultDisplay.DesiredWindowSize; } }
            public int Count { get { return _finder._resultData.Count; } }
        }
		
        private class DataSource : IDataSource
        {
            private readonly IList<T> _list;
            public DataSource(IList<T> list)
            {
                _list = list;
            }

            IndexWindow IDataSource.GetWindow(IndexWindow window)
            {
                return window.Restrict(0, _list.Count);
            }
        }
    }
}