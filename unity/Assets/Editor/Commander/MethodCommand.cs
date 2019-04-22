using System;
using System.Reflection;

namespace Pasta.Finder
{
    public class MethodCommand : ICommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SearchPath
        {
            get { return Name; }
        }
        private readonly Action _runner;
        public void Run()
        {
            _runner();
        }

        public MethodCommand(CommandAttribute attribute, MethodInfo method)
        {
            Name = attribute.Name;
            Description = attribute.Description;
            _runner = Delegate.CreateDelegate(typeof(Action), method) as Action;
        }
    }
}