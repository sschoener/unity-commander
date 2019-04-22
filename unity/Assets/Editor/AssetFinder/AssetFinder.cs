using UnityEditor;
using UnityEngine;

namespace Pasta.Finder
{
	public class AssetFinder : FinderPrompt<AssetData, AssetDataDisplay>
	{
		[MenuItem("Window/Asset Finder %k")]
		private static void Init()
		{
			var window = CreateInstance<AssetFinder>();
			window.titleContent = new GUIContent("AssetFinder");
			window.ShowFinder(
				new AssetSearchLens(),
				new AssetOpener(),
				new AssetVisualizer()
			);
		}
	}
}