using UnityEngine;

namespace Pasta.Finder
{
    public struct Hotkey
    {
        public ModifierFlags Flags { get; private set; }
        public bool Ctrl { get { return (Flags & ModifierFlags.Ctrl) != 0; } }
        public bool Shift { get { return (Flags & ModifierFlags.Shift) != 0; } }
        public bool Alt { get { return (Flags & ModifierFlags.Alt) != 0; } }
        public KeyCode Key { get; private set; }

        public Hotkey(KeyCode key, ModifierFlags flags)
        {
            Flags = flags;
            Key = key;
        }

        [System.Flags]
        public enum ModifierFlags
        {
            None = 0x0,
            Ctrl = 0x1,
            Shift = 0x2,
            Alt = 0x4
        } 
    }
}