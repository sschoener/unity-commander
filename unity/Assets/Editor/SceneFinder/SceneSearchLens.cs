using System.Collections.Generic;
using System.Linq;
using Pasta.Utilities;
using UnityEditor;
using UnityEngine;

namespace Pasta.Finder
{
    public class SceneSearchLens : ISearchLens<GameObject>
    {
        private string _searchString;
        private CachedEnumerable<GameObject> _objects;

        private static bool ShouldExploreObject(GameObject go)
        {
            return (go.hideFlags & HideFlags.HideInHierarchy) == 0;
        }

        ITerminationHandle ISearchLens<GameObject>.SetSearchData(ISelectionPosition selectionPosition,
            IConsumer<GameObject> consumer, string search)
        {
            if (_searchString == null)
                _objects = new CachedEnumerable<GameObject>(SceneWalker.SceneObjectsDFS(ShouldExploreObject));
            _searchString = search;
            if (search.Length > 0)
            {
                var matcher = SoftStringMatcher.New(search);
                consumer.Consume(_objects.Where(g => matcher.IsMatch(g.name)));
            }

            return null;
        }
    }
}