using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Pasta.Finder
{
    public class SceneObjectOpener : ISearchResultProcessor<GameObject>
    {
        private GameObject _lastObject;

        public void OnSubmit(IReadOnlyList<GameObject> results, int selection, EventModifiers modifiers)
        {
            Object[] objects;
            if ((modifiers & EventModifiers.Shift) != 0)
                objects = results.Cast<Object>().ToArray();
            else
            {
                EditorGUIUtility.PingObject(results[selection]);
                objects = new Object[] {results[selection]};
            }
            Selection.objects = objects;
            EditorAppHelpers.OpenHierarchyWindow();
        }

        public void OnSelect(IReadOnlyList<GameObject> results, int selection)
        {
            if (selection == -1) {
                _lastObject = null;
                return;
            }
                
            var obj = results[selection];
            if (obj == _lastObject)
                return;
            _lastObject = obj;
            EditorGUIUtility.PingObject(_lastObject);
        }
    }
}