using System;
using Pasta.Utilities;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace Pasta.Finder
{
    public class TextHighlighter
    {
        private readonly ObjectList<Label> _labels;
        private readonly string _highlightClass;
        private readonly VisualContainer _container;
        public VisualContainer Container { get { return _container; } }

        public TextHighlighter(string highlightClass, ILifetimeManager<Label> labels=null)
        {
            _container = new VisualContainer();
            _container.style.flexDirection = FlexDirection.Row;
            _labels = new ObjectList<Label>(
                labels ?? ListPool.New(() => new Label())
            );
            _highlightClass = highlightClass;
        }

        public void Clear()
        {
            for (int i = _labels.Count - 1; i >= 0; i--)
                _labels[i].CheapDisable();
            _labels.Clear();
        }

        public void AddNormal(string text)
        {
            var entry = _labels.AddOne();
            entry.CheapEnable();
            entry.text = text;
            entry.RemoveFromClassList(_highlightClass);
            _container.Add(entry);
        }

        public void AddHighlight(string text)
        {
            var entry = _labels.AddOne();
            entry.CheapEnable();
            entry.text = text;
            entry.AddToClassList(_highlightClass);
            _container.Add(entry);
        }
    }

    public static class TextHighlighterExtensions
    {
        public static void HighlightSubstrings(this TextHighlighter h, string str, string term, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            h.Clear();
            int i = 0;
            int f = 0;
            while ((f = str.IndexOf(term, i, comparison)) >= 0)
            {
                if (f > i)
                    h.AddNormal(str.Substring(i, f - i));
                h.AddHighlight(str.Substring(f, term.Length));
                i = f + term.Length;
                break;
            }
            h.AddNormal(str.Substring(i, str.Length - i));
        } 
    }
}