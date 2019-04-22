using UnityEditor;

namespace Pasta.Finder
{
    public static class EditorAppHelpers {
        
        public static EditorWindow OpenHierarchyWindow()
        {
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
            return EditorWindow.focusedWindow;
        }

        public static EditorWindow OpenProjectWindow()
        {
            EditorApplication.ExecuteMenuItem("Window/General/Project");
            return EditorWindow.focusedWindow;
        }
    }
}