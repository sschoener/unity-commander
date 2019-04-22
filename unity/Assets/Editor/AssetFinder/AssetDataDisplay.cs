using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace Pasta.Finder
{
    public class AssetDataDisplay : IHaveVisualElements
    {
        private readonly VisualElement _image;
        private readonly SingleHighlightText _path;
        private readonly SingleHighlightText _name;
			
        public AssetDataDisplay()
        {
            Element = new VisualElement().WithClass("result");
            Element.AddStyleSheetPath("AssetFinder");
				
            _image = new VisualElement().WithClass("asset-preview");
            Element.Add(_image);
				
            _name = new SingleHighlightText("highlight", "asset-name");
            Element.Add(_name.Container);
            _path = new SingleHighlightText("highlight", "asset-path");
            Element.Add(_path.Container);
				
        }
			
        public void Apply(SoftStringMatcher match, AssetData data)
        {
            if (data.Texture != null)
                _image.style.backgroundImage = data.Texture;
            else
                _image.style.backgroundImage = AssetDatabase.GetCachedIcon(data.Path) as Texture2D;
            _name.SetText(data.DisplayName, match.FirstMatch(data.DisplayName));
            _path.SetText(data.Path, match.LastMatch(data.Path));
        }
			
        public VisualElement Element { get; private set; }
    }
}