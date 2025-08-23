using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class ObjectUtility
    {
        public static void FindObjectOfInterface<T>(this Object _, List<T> list, bool includeInactive = false) => FindObjectOfInterface(list, includeInactive);

        public static void FindObjectOfInterface<T>(List<T> list, bool includeInactive = false)
        {
            list.Clear();
            for (int i = 0, iMax = SceneManager.sceneCount; i < iMax; ++i)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.IsValid())
                {
                    continue;
                }

                using var rootObjects = ListPool.Get<GameObject>();
                scene.GetRootGameObjects(rootObjects);
                foreach (var rootObject in rootObjects)
                {
                    using var components = ListPool.Get<T>();
                    rootObject.GetComponentsInChildren(includeInactive, components);
                    list.AddRange(components);
                }
            }
        }

        public static void FindObjectOfInterfaceInActiveScene<T>(this Object _, List<T> list, bool includeInactive = false) => FindObjectOfInterfaceInActiveScene(list, includeInactive);

        public static void FindObjectOfInterfaceInActiveScene<T>(List<T> list, bool includeInactive = false)
        {
            list.Clear();
            var scene = SceneManager.GetActiveScene();
            using var rootObjects = ListPool.Get<GameObject>();
            scene.GetRootGameObjects(rootObjects);
            foreach (var rootObject in rootObjects)
            {
                using var components = ListPool.Get<T>();
                rootObject.GetComponentsInChildren(includeInactive, components);
                list.AddRange(components);
            }
        }
    }
}