using Pasta.Utilities;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Pasta.Finder
{
    /// <summary>
    /// A generic result item with a name, a path, and an image.
    /// </summary>
    public class GenericResultItem : IHaveVisualElements
    {
        private readonly VisualElement _image;
        private readonly TextHighlighter _name;
        private readonly TextHighlighter _path;

        private static ILifetimeManager<Label> _namePoolInstance;
        private static ILifetimeManager<Label> NamePool
        {
            get
            {
                if (_namePoolInstance == null)
                {
                    _namePoolInstance = ListPool.New(
                        () => new Label().WithClass("generic-name"));
                }

                return _namePoolInstance;
            }
        }
        
        private static ILifetimeManager<Label> _pathPoolInstance;
        private static ILifetimeManager<Label> PathPool
        {
            get
            {
                if (_pathPoolInstance == null)
                {
                    _pathPoolInstance = ListPool.New(
                        () => new Label().WithClass("generic-path"));
                }

                return _pathPoolInstance;
            }
        }
			
        public GenericResultItem()
        {
            Element = new VisualElement().WithClass("result");
            Element.AddStyleSheetPath("GenericFinder");
				
            _image = new VisualElement().WithClass("generic-preview");
            Element.Add(_image);
				
            _name = new TextHighlighter("highlight", NamePool);
            _name.Container.AddToClassList("highlight-container");
            Element.Add(_name.Container);
				
				
            _path = new TextHighlighter("highlight", PathPool);
            _path.Container.AddToClassList("highlight-container");
            Element.Add(_path.Container);
				
        }
			
        public void Apply(string searchTerm, string name, string path, Texture2D icon=null)
        {
            _image.style.backgroundImage = icon;
            _path.HighlightSubstrings(path, searchTerm);
            _name.HighlightSubstrings(name, searchTerm);
        }
			
        public VisualElement Element { get; private set; }
    }
}