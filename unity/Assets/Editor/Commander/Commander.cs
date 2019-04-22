using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Pasta.Finder
{
    public class CommanderPrompt : FinderPrompt<ICommand, GenericResultItem>,
        ISearchLens<ICommand>, ISearchResultProcessor<ICommand>,
        ISearchResultDisplayer<ICommand, GenericResultItem>
    {
        [MenuItem("Window/Commander %e")]
        private static void Init()
        {
            var window = CreateInstance<CommanderPrompt>();
            window.titleContent = new GUIContent("Commander");
            window.ShowFinder(window, window, window);
        }

        public void OnSubmit(IReadOnlyList<ICommand> results, int selection, EventModifiers modifiers)
        {
            results[selection].Run();
        }

        public void OnSelect(IReadOnlyList<ICommand> results, int selection)
        {
        }

        public GenericResultItem MakeElement()
        {
            return new GenericResultItem();
        }

        public void ApplyData(string searchTerm, GenericResultItem element, IReadOnlyList<ICommand> data, int idx)
        {
            var entry = data[idx];
            element.Apply(searchTerm, entry.Name, entry.Description);
        }

        public ITerminationHandle SetSearchData(ISelectionPosition selectionPosition, IConsumer<ICommand> consumer,
            string search)
        {
            if (search.Length > 0)
            {
                var query = SearchString.Parse(search);
                var commands =
                    CommandRegistry.Instance.Commands.Where(c => StringMatcher.MatchAll(c.SearchPath, query.Base));
                consumer.Consume(commands);
            }

            return null;
        }
    }
}