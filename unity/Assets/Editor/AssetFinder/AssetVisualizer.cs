using System.Collections.Generic;

namespace Pasta.Finder
{
    public class AssetVisualizer : ISearchResultDisplayer<AssetData, AssetDataDisplay>
    {
        public AssetDataDisplay MakeElement()
        {
            return new AssetDataDisplay();
        }

        public void ApplyData(string searchTerm, AssetDataDisplay element, IReadOnlyList<AssetData> data, int idx)
        {
            var matcher = SoftStringMatcher.New(searchTerm);
            element.Apply(matcher, data[idx]);
        }
    }
}