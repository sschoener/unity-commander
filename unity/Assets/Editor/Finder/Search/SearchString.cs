using System.Collections.Generic;

namespace Pasta.Finder
{
    /// <summary>
    /// Parses search strings of the format
    /// ((base|tag:value(,value)*)ws)*
    /// where base, tag, and value are arbitrary strings and ws is whitespace. Base and value may be quoted to group
    /// them and prevent matching a ':' to start a tag.
    /// </summary>
    public class SearchString
    {
        public readonly Dictionary<string, List<string>> Tags;
        public readonly List<string> Base;

        public SearchString()
        {
            Tags = new Dictionary<string, List<string>>();
            Base = new List<string>();
        }

        public void AddTag(string tag, string value)
        {
            if (value == "")
                return;
            List<string> values;
            if (Tags.TryGetValue(tag, out values))
                values.Add(value);
            else
                Tags[tag] = new List<string> {value};
        }
        
        #region parsing
        
        public static SearchString Parse(string str)
        {
            var search = new SearchString();
            int idx = 0;
            while(idx < str.Length)
                ParseToken(str, ref idx, search);
            return search;
        }

        private static void ParseToken(string str, ref int idx, SearchString search)
        {
            while (idx < str.Length && char.IsWhiteSpace(str[idx]))
                idx++;
            if (idx >= str.Length)
                return;
            string term;
            bool isTag = false;
            if (str[idx] == '"')
            {
                // if we see a quote, the first part of the term is quoted
                term = GetQuoted(str, idx + 1);
                idx += term.Length + 2;
                if (idx < str.Length && str[idx] == ':')
                    isTag = true;
            }
            else
            {
                // otherwise, we will either see the end of the term marked by a whitespace character 
                // or by a colon.
                int end = IndexOfSeparator(str, idx);
                term = str.Substring(idx, end - idx);
                idx = end;
                if (idx < str.Length && str[idx] == ':')
                    isTag = true;
            }

            if (!isTag)
                search.Base.Add(term);
            else
            {
                search.AddTag(term, ParseTagValue(str, ref idx));
                while (idx < str.Length && str[idx] == ',')
                {
                    idx += 1;
                    search.AddTag(term, ParseTagValue(str, ref idx));
                }
            }
        }

        private static string ParseTagValue(string str, ref int idx)
        {
            if (idx >= str.Length)
                return "";
            if (str[idx] == '"')
            {
                var value = GetQuoted(str, idx + 1);
                idx += value.Length + 2;
                return value;
            }

            int end = IndexOfWhitespace(str, idx);
            int start = idx;
            idx = end + 1;
            return str.Substring(start, end - start);
        }

        /// <summary>
        /// Starting at `start`, looks for the next double quote and returns the substring up to that point (or
        /// the whole rest if there is no double quote anymore).
        /// </summary>
        /// <param name="str"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        private static string GetQuoted(string str, int start)
        {
            int idx = str.IndexOf('"', start);
            if (idx < 0)
                idx = str.Length;
            return str.Substring(start, idx - start);
        }
        
        private static int IndexOfSeparator(string str, int start)
        {
            for (int i = start; i < str.Length; i++)
            {
                if (str[i] == ':' || char.IsWhiteSpace(str[i]))
                    return i;
            }

            return str.Length;
        }

        private static int IndexOfWhitespace(string str, int start)
        {
            for (int i = start; i < str.Length; i++)
            {
                if (char.IsWhiteSpace(str[i]))
                    return i;
            }

            return str.Length;
        }
        
        #endregion
    }
}