namespace Pasta.Finder
{
    public struct IndexWindow
    {
        public readonly int Start;
        public readonly int End;
        public int Length
        {
            get { return End - Start; }
        }

        public IndexWindow(int start, int end)
        {
            Start = start;
            End = end;
        }

        private static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        private static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        public int Clamp(int idx)
        {
            return Min(End - 1, Max(Start, idx));
        }
        
        public IndexWindow Restrict(int low, int high)
        {
            int s = Start;
            int e = End;
            if (s < low)
            {
                s = low;
                e = s + Length;
            }
            if (e > high)
            {
                e = high;
                s = e - Length;
            }

            s = Max(low, s);
            e = Min(high, e);
            if (e < s)
                e = s;
            return new IndexWindow(s, e);
        }

        public override string ToString()
        {
            return "[" + Start + ":" + End + "]";
        }
    }
}