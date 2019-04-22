namespace Pasta.Finder
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string SearchPath { get; }

        void Run();
    }
}