using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Step = Pasta.Finder.IncrementalSearchStep<Pasta.Finder.AssetData, UnityEditor.HierarchyProperty, Pasta.Finder.AssetSearchLens.AssetStepDriver>;

namespace Pasta.Finder
{
    public class AssetSearchLens : ISearchLens<AssetData>
    {
        private string _filter;
        private readonly IncrementalSearch<AssetData, Step, SoftStringMatcher> _results;

        public AssetSearchLens()
        {
            _results = new IncrementalSearch<AssetData, Step, SoftStringMatcher>(new SearchDriver());
        }

        public ITerminationHandle SetSearchData(ISelectionPosition selectionPosition, IConsumer<AssetData> consumer,
            string search)
        {
            _filter = search.Trim();
            if (_filter.Length == 0)
            {
                _results.Clear();
                return null;
            }

            var matcher = SoftStringMatcher.New(_filter);
            _results.SetMatcher(matcher);
            return new SearchProcess<AssetData>(_results, selectionPosition, consumer);
        }

        private class SearchDriver : IIncrementalSearchDriver<Step, SoftStringMatcher>
        {
            ComparisonResult IIncrementalSearchDriver<Step, SoftStringMatcher>.CompareSpecificity(
                SoftStringMatcher left, SoftStringMatcher right)
            {
                return left.CompareTo(right);
            }

            SoftStringMatcher IIncrementalSearchDriver<Step, SoftStringMatcher>.GetFilter(Step searcher)
            {
                return searcher.Driver.Matcher;
            }

            Step IIncrementalSearchDriver<Step, SoftStringMatcher>.GetRoot(SoftStringMatcher filter)
            {
                return new Step(new AssetStepDriver(filter, null), null);
            }

            Step IIncrementalSearchDriver<Step, SoftStringMatcher>.GetSub(SoftStringMatcher filter,
                Step parent)
            {
                return new Step(
                    new AssetStepDriver(filter, parent.Driver.HierarchyProperty),
                    parent
                );
            }
        }

        public class AssetStepDriver : ISearchStepDriver<AssetData, HierarchyProperty>
        {
            public readonly HierarchyProperty HierarchyProperty;
            public readonly SoftStringMatcher Matcher;

            public AssetStepDriver(SoftStringMatcher matcher, HierarchyProperty parent)
            {
                Matcher = matcher;
                if (parent != null && !parent.isValid)
                    HierarchyProperty = parent;
                else
                {
                    HierarchyProperty = new HierarchyProperty(HierarchyType.Assets);
                    if (parent != null)
                    HierarchyProperty.Find(parent.instanceID, null);
                }    
            }
            
            int ISearchStepDriver<AssetData, HierarchyProperty>.BatchSize { get { return 25; } }

            private static IEnumerable<HierarchyProperty> Enumerate(HierarchyProperty p)
            {
                p.SetSearchFilter(null, 0);
                while (p.Next(null))
                {
                    if (p.isFolder)
                        continue;
                    yield return p;
                }
            }
            
            IEnumerable<HierarchyProperty> ISearchStepDriver<AssetData, HierarchyProperty>.EnumerateNew()
            {
                return Enumerate(HierarchyProperty);
            }

            bool ISearchStepDriver<AssetData, HierarchyProperty>.FilterCached(AssetData value)
            {
                return Matcher.IsMatchFromEnd(value.DisplayName);
            }

            bool ISearchStepDriver<AssetData, HierarchyProperty>.FilterNew(HierarchyProperty value)
            {
                return Matcher.IsMatchFromEnd(value.name);
            }

            AssetData ISearchStepDriver<AssetData, HierarchyProperty>.MakeData(HierarchyProperty p)
            {
                string path = AssetDatabase.GUIDToAssetPath(p.guid);
                Texture2D icon = null;
                if (!p.isMainRepresentation)
                    icon = p.icon;
                return new AssetData
                {
                    Texture = icon,
                    Path = path,
                    DisplayName = p.name,
                    InstanceID = p.instanceID
                };
            }
        }
    }
}