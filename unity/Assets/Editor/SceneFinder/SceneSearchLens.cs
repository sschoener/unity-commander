using System.Collections.Generic;
using System.Linq;
using Pasta.Utilities;
using UnityEditor;
using UnityEngine;
using Step = Pasta.Finder.IncrementalSearchStep<UnityEngine.GameObject, UnityEngine.GameObject, Pasta.Finder.SceneSearchLens.SceneSearchStepDriver>;

namespace Pasta.Finder
{
    public class SceneSearchLens : ISearchLens<GameObject>
    {
        private string _searchString;
        private CachedEnumerable<GameObject> _objects;
        private IncrementalSearch<GameObject, Step, SoftStringMatcher> _results;

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
                return new Step(new SceneSearchStepDriver(filter, null), null);
            }

            Step IIncrementalSearchDriver<Step, SoftStringMatcher>.GetSub(SoftStringMatcher filter,
                Step parent)
            {
                return new Step(
                    new SceneSearchStepDriver(filter, new SceneTransformIterator(parent.Driver.Iterator)),
                    parent
                );
            }
        }

        public class SceneSearchStepDriver : ISearchStepDriver<GameObject, GameObject>
        {
            public readonly SoftStringMatcher Matcher;
            public readonly SceneTransformIterator Iterator;

            public SceneSearchStepDriver(SoftStringMatcher matcher, SceneTransformIterator iter)
            {
                Matcher = matcher;
                Iterator = iter;
            }
            
            int ISearchStepDriver<GameObject, GameObject>.BatchSize { get { return 25; } }
            
            IEnumerable<GameObject> ISearchStepDriver<GameObject, GameObject>.EnumerateNew()
            {
                while (Iterator.MoveNext())
                    yield return Iterator.Current.gameObject;
            }

            bool ISearchStepDriver<GameObject, GameObject>.FilterCached(GameObject value)
            {
                return Matcher.IsMatchFromEnd(value.name);
            }

            bool ISearchStepDriver<GameObject, GameObject>.FilterNew(GameObject value)
            {
                return Matcher.IsMatchFromEnd(value.name);
            }

            GameObject ISearchStepDriver<GameObject, GameObject>.MakeData(GameObject p)
            {
                return p;
            }
        }
    }
}