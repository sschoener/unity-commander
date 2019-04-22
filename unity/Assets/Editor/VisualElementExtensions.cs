using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace Pasta.Finder
{
    public static class VisualElementExtensions
    {
        public static T WithClass<T>(this T t, string className) where T : VisualElement
        {
            t.AddToClassList(className);
            return t;
        }

        /// <summary>
        /// Disabling a visual element with this method requires that its clipping mode is set properly.
        /// By using this method, you will eventually have garbage-free disable/enable and only pay the cost of
        /// relayouting (+clipping) - and that you would have to pay anyway.
        /// </summary>
        /// <param name="ve"></param>
        public static void CheapDisable(this VisualElement ve)
        {
            ve.style.positionType = PositionType.Manual;
            ve.style.height = 0;
            ve.style.width = 0;
            ve.visible = false;
        }

        public static void CheapEnable(this VisualElement ve)
        {
            ve.visible = true;
            ve.ResetPositionProperties();
        }
    }
}