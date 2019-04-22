using System.Collections.Generic;
using Pasta.Utilities;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEngine.Experimental.UIElements;

namespace Pasta.Finder
{
	public class FinderPrompt<T, V> : EditorWindow where V : IHaveVisualElements
	{
		private TextField _searchField;
		private Label _infoLabel;
		private Finder<T, V> _finder;

		private bool _closeOnFocusLost = true;
		public bool CloseOnFocusLost
		{
			get { return _closeOnFocusLost; }
			set { _closeOnFocusLost = value; }
		}
	
		public void ShowFinder(
			ISearchLens<T> searchLens,
			ISearchResultProcessor<T> processor,
			ISearchResultDisplayer<T, V> display
		)
		{
			_finder = new Finder<T, V>(
				_searchField,
				searchLens,
				new CloseProcessor(this, processor),
				display
			);
			var root = this.GetRootVisualContainer();
			root.Add(_finder.Container);
			_finder.Container.PlaceBehind(_infoLabel);
			_finder.Container.name = "ResultList";

			_finder.SearchStateChanged += OnSearchStateChanged;
			
			minSize = new Vector2(1, 1);
			ShowPopup();
			Focus();
		}
		
		private void OnEnable()
		{
			var root = this.GetRootVisualContainer();
			root.name = "Root";
			root.AddStyleSheetPath("Finder");
					
			_searchField = SetupSearchField();
			root.Add(_searchField);
			_infoLabel = new Label();
			_infoLabel.name = "ResultInfo";
			_infoLabel.CheapDisable();
			root.Add(_infoLabel);
			root.schedule.Execute(UpdateWindowSize).Every(1);
		}
		
		private void OnSearchStateChanged(SearchProgress search)
		{
			if (search.NoSearch)
			{
				_infoLabel.CheapDisable();
				return;
			}
			_infoLabel.CheapEnable();
			int n = search.NumResults;
			var label = n.ToString();
			if (search.InProgress)
				label += "+";
			if (n != 1 || search.InProgress)
				_infoLabel.text = label + " results";
			else
				_infoLabel.text = label + " result";
		}

		private TextField SetupSearchField()
		{
			var searchField = new TextField()
			{
				name = "SearchField",
				style =
				{
					unityTextAlign = TextAnchor.MiddleLeft,
				}
			};
			searchField.RegisterCallback<KeyDownEvent>(HandleSearchFieldInputs);
			return searchField;
		}
		
		private void HandleSearchFieldInputs(KeyDownEvent e)
		{
			switch (e.keyCode)
			{
				case KeyCode.Escape:
					DoClose();
					break;
				default:
					return;
			}
			e.PreventDefault();
			e.StopPropagation();
		}

		private void UpdateWindowSize()
		{
			var root = this.GetRootVisualContainer();
			float minHeight = root.style.minHeight;
			float minWidth = root.style.minWidth;
			var pos = position;
			pos.width = minWidth;
			pos.height = Mathf.Max(minHeight, root.layout.height);
			pos.xMin = Screen.currentResolution.width / 2f - pos.width / 2;
			pos.yMin = Screen.currentResolution.height * .3f;
			position = pos;
		}

		private void OnFocus()
		{
			// when the window is focused, focus the search bar
			_searchField.Focus();
		}
		
		private void OnLostFocus()
		{
			if (CloseOnFocusLost)
				DoClose();
		}

		private void DoClose()
		{
			if (_finder != null)
				_finder.Stop();
			Close();
		}
		
		private void OnDestroy()
		{
			if (_finder != null)
				_finder.Stop();
		}

		private class CloseProcessor : ISearchResultProcessor<T>
		{
			private readonly ISearchResultProcessor<T> _inner;
			private readonly FinderPrompt<T, V> _finder;
			public CloseProcessor(FinderPrompt<T, V> finder, ISearchResultProcessor<T> proc)
			{
				_finder = finder;
				_inner = proc;
			}
			
			public void OnSubmit(IReadOnlyList<T> results, int selection, EventModifiers modifiers)
			{
				_inner.OnSubmit(results, selection, modifiers);
				_finder.DoClose();
			}

			public void OnSelect(IReadOnlyList<T> results, int selection)
			{
				_inner.OnSelect(results, selection);
			}
		}
	}
}