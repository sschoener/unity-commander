using System;

namespace Pasta.Finder
{
    /// <summary>
    /// Tag a method with this attribute to make it visible to the command finder.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class CommandAttribute : Attribute
    {
        public readonly string Name;
        public readonly string Description;

        public CommandAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}