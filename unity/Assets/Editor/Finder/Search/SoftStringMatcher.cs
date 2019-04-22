using System.Collections.Generic;

namespace Pasta.Finder
{
    public struct SoftStringMatcher
    {
        private readonly List<char> _normalizedString;

        private SoftStringMatcher(List<char> str)
        {
            _normalizedString = str;
        }

        public ComparisonResult CompareTo(SoftStringMatcher other)
        {
            int otherCount = other._normalizedString.Count;
            int thisCount = _normalizedString.Count;
            if (otherCount == thisCount)
            {
                for (int i = 0; i < thisCount; i++)
                {
                    if (_normalizedString[i] != other._normalizedString[i])
                        return ComparisonResult.Incomparable;
                }

                return ComparisonResult.Equal;
            }

            if (thisCount < otherCount)
                return IsMatch(other._normalizedString) ? ComparisonResult.Less : ComparisonResult.Incomparable;
            return other.IsMatch(_normalizedString) ? ComparisonResult.Greater : ComparisonResult.Incomparable;
        }

        public bool IsLessSpecificThan(SoftStringMatcher other)
        {
            return CompareTo(other) == ComparisonResult.Less;
        }

        private bool IsMatch(IReadOnlyList<char> other)
        {
            int start = -1;
            int needle = 0;
            bool inSeparator = false;
            for (int i = 0; i < other.Count; i++)
            {
                if (Advance(other[i], ref i, ref start, ref needle, ref inSeparator))
                    return true;
            }
            return false;
        }

        public static SoftStringMatcher New(string str)
        {
            return new SoftStringMatcher(Normalize(str));
        }

        private static bool IsSeparator(char c)
        {
            return c == '_' || c == '-' || char.IsWhiteSpace(c);
        }

        private static List<char> Normalize(string str)
        {
            str = str.Trim();
            var norm = new List<char>(str.Length);
            bool inSeparator = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (IsSeparator(str[i]))
                {
                    if (inSeparator) continue;
                    norm.Add(str[i]);
                    inSeparator = true;
                }
                else
                {
                    inSeparator = false;
                    norm.Add(char.ToLowerInvariant(str[i]));
                }
            }

            return norm;
        }

        private bool CheckMatch(char c, ref int needle, ref bool inSeparator, bool backwards=false)
        {
            int idx = backwards ? _normalizedString.Count - 1 - needle : needle; 
            if (IsSeparator(c))
            {
                // if it is a separator, count only the first
                if (inSeparator)
                    return true;
                // else match against a whitespace
                return inSeparator = _normalizedString[idx] == ' ';
            }

            // otherwise we may ignore any whitespace in the normalized string
            if (_normalizedString[idx] == ' ')
            {
                // this works because the normalized string never starts/begins with whitespace
                needle++;
                idx = backwards ? idx - 1 : idx + 1;
            }
            inSeparator = false;
            return char.ToLowerInvariant(c) == _normalizedString[idx];
        }

        private bool Advance(char c, ref int i, ref int start, ref int needle, ref bool inSeparator, bool backwards=false)
        {
            if (CheckMatch(c, ref needle, ref inSeparator, backwards))
            {
                if (needle == 0)
                    start = i;
                needle++;
                if (needle >= _normalizedString.Count)
                    return true;
            }
            else if (needle > 0)
            {
                needle = 0;
                i = start;
            }

            return false;
        }
        
        public StringMatch FirstMatch(string str, int fromIdx = 0)
        {
            int start = -1;
            int needle = 0;
            bool inSeparator = false;
            for (int i = fromIdx; i < str.Length; i++)
            {
                if (Advance(str[i], ref i, ref start, ref needle, ref inSeparator))
                    return new StringMatch(start, i - start + 1);
            }

            return StringMatch.Invalid;
        }

        public StringMatch LastMatch(string str)
        {
            return LastMatch(str, str.Length - 1);
        }
        
        public StringMatch LastMatch(string str, int fromIdx)
        {
            int start = -1;
            int needle = 0;
            bool inSeparator = false;
            for (int i = fromIdx; i >= 0; i--)
            {
                if (Advance(str[i], ref i, ref start, ref needle, ref inSeparator, backwards: true))
                    return new StringMatch(i, start - i + 1);
            }

            return StringMatch.Invalid;
        }

        public bool IsMatch(string str)
        {
            return FirstMatch(str).IsValid;
        }

        public bool IsMatchFromEnd(string str)
        {
            return LastMatch(str).IsValid;
        }

        public override string ToString()
        {
            return "Matcher { " + new string(_normalizedString.ToArray()) + " }";
        }
    }
}