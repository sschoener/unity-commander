using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Pasta.Finder
{  
    public class CommandRegistry
    {
        public readonly IEnumerable<ICommand> Commands;
        
        private CommandRegistry()
        {
            var commands = FindAllCommands().ToList();
            commands.Sort((l, r) => l.Name.CompareTo(r.Name));
            Commands = commands;
        }
        
        public static CommandRegistry Instance { get; private set; }
        [InitializeOnLoadMethod]
        public static void Load()
        {
            Instance = new CommandRegistry();
        }

        private static IEnumerable<ICommand> FindAllCommands()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var methods = assemblies.SelectMany(
                a => a.GetTypes().SelectMany(
                    t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                )
            );
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(inherit: false);
                for (int i = 0; i < attributes.Length; i++)
                {
                    var menuItem = attributes[i] as MenuItem;
                    if (menuItem != null)
                    {
                        string name = menuItem.menuItem;
                        if (menuItem.validate || name.StartsWith("internal:") || name.StartsWith("CONTEXT/"))
                            continue;
                        
                        yield return new MenuItemCommand(menuItem);
                        continue;
                    }

                    var command = attributes[i] as CommandAttribute;
                    if (command != null)
                        yield return new MethodCommand(command, method);
                }
            }
        }
    }
}