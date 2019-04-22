using System.Collections.Generic;
using System.Linq;
using Pasta.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Pasta.Finder
{   
    public class DebugFinder : FinderPrompt<string, DebugFinder.Entry>, ISearchResultProcessor<string>, ISearchResultDisplayer<string, DebugFinder.Entry>, ISearchLens<string>
    {
	    public class Entry : IHaveVisualElements
		{
			public readonly TextElement Text;
			public VisualElement Element { get { return Text; } }

			public Entry()
			{
				Text = new TextElement();
				Text.AddToClassList("result");
			}
		}
	    
		public Entry MakeElement()
		{
			return new Entry();
		}

	    public void ApplyData(string searchTerm, Entry element, IReadOnlyList<string> data, int idx)
	    {
		    element.Text.text = data[idx];
	    }
	    
	    [MenuItem("Window/Debug Finder")]
		private static void Init()
		{
			var window = CreateInstance<DebugFinder>();
			window.titleContent = new GUIContent("DebugFinder");
			window.ShowFinder(window, window, window);
		}
		
	    void ISearchResultProcessor<string>.OnSubmit(IReadOnlyList<string> results, int selection, EventModifiers modifiers)
	    {
		    Debug.Log("Submit: " + results[selection]);
	    }

	    void ISearchResultProcessor<string>.OnSelect(IReadOnlyList<string> results, int selection)
	    {
		    Debug.Log("Select: " + results[selection]);
	    }

	    ITerminationHandle ISearchLens<string>.SetSearchData(ISelectionPosition selectionPosition, IConsumer<string> consumer, string search)
	    {
			consumer.Consume(search.Select(c => c.ToString()));
		    return Terminated.Instance;
	    }
    }
}