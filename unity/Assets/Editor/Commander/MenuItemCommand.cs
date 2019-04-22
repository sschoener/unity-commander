using UnityEditor;

namespace Pasta.Finder
{
    public class MenuItemCommand : ICommand
    {
        private readonly string _fullName;
        private readonly string _path;
        private readonly Hotkey _hotkey;

        public MenuItemCommand(MenuItem item)
        {
            _fullName = item.menuItem;
            string name = item.menuItem;
            int pathEnd = name.LastIndexOf('/');
            bool hasPath = pathEnd >= 0;
            if (hasPath)
            {
                _path = name.Substring(0, pathEnd);
                name = name.Substring(pathEnd + 1, name.Length - pathEnd - 1);
            }

            int hotkeyStart = name.LastIndexOf(' ');
            if (hotkeyStart >= 0 && IsHotkeyStart(name, hotkeyStart + 1))
                name = name.Substring(0, hotkeyStart);

            Name = name;
            if (!hasPath)
                _path = Name;
        }

        private static bool IsHotkeyStart(string name, int idx)
        {
            if (idx >= name.Length)
                return false;
            var c = name[idx];
            return c == '_' || c == '%' || c == '#' || c == '&';
        }

        public string Name { get; private set; }
        public string Description { get { return _path; } }
        public string SearchPath { get { return _fullName; } }
        
        public void Run()
        {
            EditorApplication.ExecuteMenuItem(_fullName);
        }
    }
}