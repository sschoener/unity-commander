namespace Pasta.Finder
{
    public static class DataSourceExtensions {
        public static IndexWindow GetWindow(this IDataSource source, int start, int end)
        {
            return source.GetWindow(new IndexWindow(start, end));
        }
    }
}