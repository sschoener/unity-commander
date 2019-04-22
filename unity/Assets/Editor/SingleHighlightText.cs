using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace Pasta.Finder
{
    public class SingleHighlightText
    {
        public VisualContainer Container { get; private set; }
        public Label FirstPart { get; private set; }
        public Label SecondPart { get; private set; }
        public Label ThirdPart { get; private set; }

        public SingleHighlightText(string highlightClass, string labelClass)
        {
            Container = new VisualContainer();
            Container.style.flexDirection = FlexDirection.Row;
            Container.WithClass("highlight-container");
            FirstPart = new Label().WithClass(labelClass);
            SecondPart = new Label().WithClass(highlightClass).WithClass(labelClass);
            ThirdPart = new Label().WithClass(labelClass);
            Container.Add(FirstPart);
            Container.Add(SecondPart);
            Container.Add(ThirdPart);
        }

        public void SetText(string text, int highlightStart, int highlightLength)
        {
            if (highlightLength == 0 || highlightStart >= text.Length)
            {
                FirstPart.text = text;
                SecondPart.CheapDisable();
                ThirdPart.CheapDisable();
            }
            else
            {
                FirstPart.text = text.Substring(0, highlightStart);
                SecondPart.CheapEnable();
                SecondPart.text = text.Substring(highlightStart, highlightLength);
                int end = highlightStart + highlightLength;
                if (end < text.Length)
                {
                    ThirdPart.CheapEnable();
                    ThirdPart.text = text.Substring(end, text.Length - end);
                }
                else
                    ThirdPart.CheapDisable();
            }
        }
    }

    public static class SingleHighlightTextExtensions
    {
        public static void SetText(this SingleHighlightText h, string text, StringMatch match)
        {
            h.SetText(text, match.Start, match.Length);
        }
    }
}