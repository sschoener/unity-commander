using System.Collections.Generic;
using UnityEditor;

namespace Pasta.Finder
{
    public static class AssetSearch
    {
        public static IEnumerable<HierarchyProperty> EnumerateAssets()
        {
            return EnumerateAssets(new HierarchyProperty(HierarchyType.Assets), p => p, p => true);
        }
        
        public static IEnumerable<HierarchyProperty> EnumerateAssets(HierarchyProperty property)
        {
            return EnumerateAssets(property, p => p, p => true);
        }
        
        public static IEnumerable<T> EnumerateAssets<T>(HierarchyProperty property,
                                                        System.Func<HierarchyProperty, T> selector)
        {
            return EnumerateAssets(property, selector, p => true);
        }
        
        public static IEnumerable<T> EnumerateAssets<T>(HierarchyProperty property,
                                                        System.Func<HierarchyProperty, T> selector,
                                                        System.Predicate<HierarchyProperty> includeChildren)
        {
            var p = property;
            // construct an empty search filter
            p.SetSearchFilter(null, 0);
            int maxDepth = int.MaxValue;
            do
            {
                if (p.depth > maxDepth)
                    continue;
                if (p.hasChildren)
                    maxDepth = !includeChildren(p) ? p.depth : int.MaxValue;
                if (!p.isFolder)
                    yield return selector(p);
                // pass null to indicate that we traverse all children, not just a subset of expanded ones
            } while (p.Next(null));
        }
    }
}