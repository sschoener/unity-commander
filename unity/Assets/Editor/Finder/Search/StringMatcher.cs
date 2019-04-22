using System;
using System.Collections.Generic;
using System.Linq;

namespace Pasta.Finder
{
    public static class StringMatcher
    { 
        public static int CountMatches(string s, List<string> query)
        {
            int matches = 0;
            for (int i = 0; i < query.Count; i++)
            {
                if (s.IndexOf(query[i], StringComparison.OrdinalIgnoreCase) >= 0)
                    matches++;
            }

            return matches;
        }
        
        public static bool MatchAny(string s, List<string> query)
        {
            for (int i = 0; i < query.Count; i++)
            {
                if (s.IndexOf(query[i], StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        public static bool MatchAll(string s, List<string> query)
        {
            for (int i = 0; i < query.Count; i++)
            {
                if (s.IndexOf(query[i], StringComparison.OrdinalIgnoreCase) < 0)
                    return false;
            }
            return true;
        }
    }
}