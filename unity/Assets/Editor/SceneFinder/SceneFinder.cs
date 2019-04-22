using UnityEditor;
using UnityEngine;

namespace Pasta.Finder
{
	public class SceneFinder : FinderPrompt<GameObject, GenericResultItem>
    {
		[MenuItem("Window/Scene Finder %#k")]
		private static void Init()
		{
			var window = CreateInstance<SceneFinder>();
			window.titleContent = new GUIContent("SceneFinder");
			window.ShowFinder(
				new SceneSearchLens(),
				new SceneObjectOpener(),
				new SceneObjectVisualizer()
			);
		}
    }
}