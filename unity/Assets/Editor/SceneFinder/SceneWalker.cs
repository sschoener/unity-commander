using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pasta.Finder
{
    public static class SceneWalker
    {
        public static List<GameObject> SceneRoots()
        {
            var roots = new List<GameObject>();
            int scenes = SceneManager.sceneCount;
            for (int i = 0; i < scenes; i++)
                SceneManager.GetSceneAt(i).GetRootGameObjects(roots);
            return roots;
        }
        
        public static IEnumerable<GameObject> SceneObjects(Predicate<GameObject> explore)
        {
            var queue = new Queue<GameObject>();
            var roots = new List<GameObject>();
            int scenes = SceneManager.sceneCount;
            for (int i = 0; i < scenes; i++)
            {
                SceneManager.GetSceneAt(i).GetRootGameObjects(roots);
                for (int j = 0; j < roots.Count; j++)
                    queue.Enqueue(roots[j]);
                roots.Clear();
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;
                if (!explore(current))
                    continue;
                int c = current.transform.childCount;
                for (int i = 0; i < c; i++)
                    queue.Enqueue(current.transform.GetChild(i).gameObject);
            }
        }
        
        public static IEnumerable<GameObject> SceneObjectsDFS(Predicate<GameObject> explore)
        {
            var queue = new Queue<GameObject>();
            var roots = new List<GameObject>();
            int scenes = SceneManager.sceneCount;
            for (int i = 0; i < scenes; i++)
            {
                SceneManager.GetSceneAt(i).GetRootGameObjects(roots);
                for (int j = 0; j < roots.Count; j++)
                    queue.Enqueue(roots[j]);
                roots.Clear();
            }

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;
                if (!explore(current))
                    continue;
                int c = current.transform.childCount;
                for (int i = 0; i < c; i++)
                    queue.Enqueue(current.transform.GetChild(i).gameObject);
            }
        }
    }
}