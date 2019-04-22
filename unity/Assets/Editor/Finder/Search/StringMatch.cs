namespace Pasta.Finder
{
    public struct StringMatch
    {
        public readonly int Start;
        public readonly int Length;

        public StringMatch(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public static StringMatch Invalid
        {
            get { return new StringMatch(-1, 0); }
        }
        
        public bool IsValid
        {
            get { return Start >= 0; }
        }

        public override string ToString()
        {
            if (!IsValid)
                return "StringMatch { Invalid }";
            return "StringMatch { " + Start + ", " + (Start + Length) + "}";
        }
    }
}