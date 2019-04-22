using UnityEditor;
using UnityEditor.SceneManagement;

namespace Pasta.Finder
{
    public static class TestCommands
    {
        [Command("Build Settings", "Shows the build settings")]
        public static void BuildSettings()
        {
            EditorApplication.ExecuteMenuItem("File/Build Settings...");
        }

        [Command("New Scene", "Creates a new scene")]
        public static void NewScene()
        {
            EditorApplication.ExecuteMenuItem("File/New Scene");
        }

        [Command("Open Scene", "Opens a scene")]
        public static void OpenScene()
        {
            EditorApplication.ExecuteMenuItem("File/Open Scene");
        }
        
        [Command("Save Scene", "Saves the current scene")]
        public static void SaveScene()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }
        
        [Command("Save Scene As", "Saves the current scene with a new name")]
        public static void SaveSceneAs()
        {
            EditorApplication.ExecuteMenuItem("File/Save As...");
        }
        
        [Command("New Project", "Creates a new project")]
        public static void NewProject()
        {
            EditorApplication.ExecuteMenuItem("File/New Project");
        }
        
        [Command("Open Project", "Opens a project")]
        public static void OpenProject()
        {
            EditorApplication.ExecuteMenuItem("File/Open Project...");
        }
        
        [Command("Save Project", "Saves the current project")]
        public static void SaveProject()
        {
            EditorApplication.ExecuteMenuItem("File/Save Project");
        }

        [Command("Build and Run", "Builds and runs the current project")]
        public static void BuildAndRun()
        {
            EditorApplication.ExecuteMenuItem("File/Build And Run");
        }
    }
}