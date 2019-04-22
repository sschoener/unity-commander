using System.Collections.Generic;
using System.Linq;
using Pasta.Utilities;
using UnityEditor;
using UnityEngine;

namespace Pasta.Finder
{
    public class AssetOpener : ISearchResultProcessor<AssetData>
    {
        private readonly bool _jumpOnSelection;
        private AssetData _lastAsset;
        public AssetOpener(bool jumpOnSelection=false)
        {
            _jumpOnSelection = jumpOnSelection;
        }

        public void OnSubmit(IReadOnlyList<AssetData> results, int selection, EventModifiers modifiers)
        {
            int[] assets;
            // should we select all results or just one of them?
            if ((modifiers & EventModifiers.Shift) != 0)
                assets = results.Select(d => d.InstanceID).ToArray();
            else
            {
                int id = results[selection].InstanceID;
                EditorGUIUtility.PingObject(id);
                assets = new[] {id};
            }

            Selection.instanceIDs = assets;
            EditorAppHelpers.OpenProjectWindow();
            if ((modifiers & EventModifiers.Control) != 0)
                return;
            // and open them if ctrl is not down
            AssetDatabase.OpenAsset(assets[0]);
        }

        public void OnSelect(IReadOnlyList<AssetData> results, int selection)
        {
            if (!_jumpOnSelection) return;
            var data = results[selection];
            if (_lastAsset.InstanceID == data.InstanceID)
                return;
            _lastAsset = data;
            EditorGUIUtility.PingObject(data.InstanceID);
        }
    }
}