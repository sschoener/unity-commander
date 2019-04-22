
namespace Pasta.Finder
{
    public class SlidingSelectionWindow
    {
        private readonly IDataSource _dataSource;

        public IndexWindow Window { get; private set; }
        public int SelectionIndex { get; private set; }

        public int DesiredWindowSize { get; private set; }
        public int EffectiveWindowSize
        {
            get { return Window.Length; }
        }
        public int SelectionPositionInWindow { get; private set; }

        public SlidingSelectionWindow(IDataSource dataSource,
            int desiredWindowSize, int selectionPositionInWindow)
        {
            _dataSource = dataSource;
            DesiredWindowSize = desiredWindowSize;
            SelectionPositionInWindow = selectionPositionInWindow;
            if (SelectionPositionInWindow >= DesiredWindowSize)
                throw new System.ArgumentException("The position in the window must be smaller than the window size!");
            JumpTo(0);
        }

        private static int Max(int a, int b)
        {
            return a > b ? a : b;
        }
        
        private void Apply(int start, int selection)
        { 
            Window = _dataSource.GetWindow(start, start + DesiredWindowSize);
            SelectionIndex = Window.Clamp(selection);
        }
        
        public void JumpTo(int selection)
        {
            int start = Max(selection - SelectionPositionInWindow, 0);
            Apply(start, selection);
        }

        public void Scroll(int amount)
        {
            var window = _dataSource.GetWindow(Window.Start, Window.Start + DesiredWindowSize);
            int selection = SelectionIndex + amount;
            if (selection >= window.End || selection < window.Start)
                Apply(Window.Start + amount, selection);
            else
                Apply(Window.Start, selection);
        }

        public void Refresh()
        {
            Scroll(0);
        }
    }
}