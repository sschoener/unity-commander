using UnityEngine;

namespace Pasta.Finder
{
    public struct AssetData
    {
        public Texture2D Texture;
        public int InstanceID;
        public bool IsSub
        {
            get { return Texture != null; }
        }
        public string DisplayName;
        public string Path;

        public string DisplayPath
        {
            get { return !IsSub ? Path : Path + ":" + DisplayName; }
        }
    }
}