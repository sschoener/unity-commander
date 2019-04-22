using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Pasta.Finder
{
    public class SceneObjectVisualizer : ISearchResultDisplayer<GameObject, GenericResultItem>
    {
        public GenericResultItem MakeElement()
        {
            return new GenericResultItem();
        }

        public void ApplyData(string searchTerm, GenericResultItem element, IReadOnlyList<GameObject> data, int idx)
        {
            var entry = data[idx];
            element.Apply(searchTerm, entry.name, GetPath(entry), AssetPreview.GetMiniThumbnail(entry));
        }

        private string GetPath(GameObject data)
        {
            var sb = new StringBuilder();
            var parents = new List<GameObject>();
            var current = data;
            while (current.transform.parent != null)
            {
                current = current.transform.parent.gameObject;
                parents.Add(current);
            }

            for (int i = parents.Count - 1; i >= 0; i--)
            {
                sb.Append(parents[i].name);
                sb.Append('/');
            }

            return sb.ToString();
        }
    }
}