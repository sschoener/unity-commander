using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Pasta.Finder
{
    
    public class ReuseSelectionList<V> where V : IHaveVisualElements
    {
        private readonly ReuseList<V> _resultDisplay;
        private readonly ApplyData _displayer;
        private readonly SlidingSelectionWindow _selectionWindow;

        public delegate void ApplyData(V element, int index);
        
        public VisualContainer Container
        {
            get { return _resultDisplay.Container;  }
        }
        
        public int SelectionIndex
        {
            get { return _selectionWindow.SelectionIndex; }
        }
        
        public int DesiredWindowSize
        {
            get { return _selectionWindow.DesiredWindowSize;  }
        }
		
        public ReuseSelectionList(Func<V> spawner,
            ApplyData displayer,
            IDataSource source,
            int windowSize,
            int selectionIndexInWindow)
        {
            _resultDisplay = new ReuseList<V>(spawner);
            _displayer = displayer;
            _selectionWindow = new SlidingSelectionWindow(source, windowSize, selectionIndexInWindow);
        }

        public void Prewarm()
        {
            _resultDisplay.SetSize(_selectionWindow.DesiredWindowSize);
            _resultDisplay.SetSize(0);
        }

        public void CheckForNew()
        {
            int previousWindowSize = _selectionWindow.EffectiveWindowSize;
            if (previousWindowSize == _selectionWindow.DesiredWindowSize)
                return;
            _selectionWindow.Refresh();
            IncrementalUpdate(previousWindowSize);
        }

        public void JumpTo(int index)
        {
            SetSelectionActive(false);
            _selectionWindow.JumpTo(index);
            FullUpdate();
        }
		
        public void Scroll(int amount)
        {
            if (amount == 0 || _selectionWindow.EffectiveWindowSize == 0)
                return;
            SetSelectionActive(false);
            int oldStart = _selectionWindow.Window.Start;
            _selectionWindow.Scroll(amount);
            int delta = oldStart - _selectionWindow.Window.Start;
            int size = _selectionWindow.EffectiveWindowSize;
            if (delta <= -size || delta >= size)
                FullUpdate();
            else
            {
                // otherwise we can update the entries more efficiently by simply cycling them and only updating
                // the new entries.
                _resultDisplay.CycleForward(delta);
                if (delta < 0)
                    UpdateResultsDisplay(size + delta, size);
                else
                    UpdateResultsDisplay(0, delta);
            }
            SetSelectionActive(true);
        }

        
        private int LocalSelectionIndex
        {
            get { return _selectionWindow.SelectionIndex - _selectionWindow.Window.Start; }
        }
		
        private void SetSelectionActive(bool active)
        {
            int idx = LocalSelectionIndex;
            if (idx >= 0 && idx < _resultDisplay.Size)
                _resultDisplay[idx].Element.EnableInClassList("selected", active);
        }

        private void UpdateResultsDisplay(int from, int to)
        {
            if (from < 0)
                from = 0;
            if (to > _selectionWindow.EffectiveWindowSize)
                to = _selectionWindow.EffectiveWindowSize;
            for (int i = from; i < to; i++)
            {
                int o = i + _selectionWindow.Window.Start;
                var className = o % 2 == 0 ? "even" : "odd";
                var otherClassName = o % 2 == 0 ? "odd" : "even";
                _resultDisplay[i].Element.RemoveFromClassList(otherClassName);
                _resultDisplay[i].Element.AddToClassList(className);
                _displayer(_resultDisplay[i], o);
            }
        }

        private void IncrementalUpdate(int oldSize)
        {
            int newSize = _selectionWindow.EffectiveWindowSize;
            if (oldSize == newSize)
                return;
            if (newSize == 0)
            {
                _resultDisplay.Container.CheapDisable();
                return;
            }
            if (oldSize <= 0)
                _resultDisplay.Container.CheapEnable();
            _resultDisplay.SetSize(newSize);
            UpdateResultsDisplay(oldSize, newSize);
        }
        
        private void FullUpdate()
        {
            SetSelectionActive(false);
            IncrementalUpdate(-1);
            SetSelectionActive(true);
        }
    }
}