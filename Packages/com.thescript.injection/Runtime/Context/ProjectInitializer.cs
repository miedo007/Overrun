using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mtl.ContextManagement
{
    internal class ProjectInitializer
    {
        internal static ProjectInitializer Instance;

        private const string ProjectFilename = "ProjectContext";
        private bool _initialized;
        private Object _projectContext;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadInstance()
        {
            Instance = new ProjectInitializer();
        }

        public void Initialize(Action onComplete)
        {
            if (!_initialized)
            {
                var prefab = Resources.Load(ProjectFilename, typeof(GameObject));
                if (prefab == null)
                {
                    throw new Exception($"Cannot find {ProjectFilename} file in Resources");
                }

                _initialized = true;
                _projectContext = Object.Instantiate(prefab);
                Object.DontDestroyOnLoad(_projectContext);
            }

            onComplete?.Invoke();
        }

        public void NotifyContextDestroyed(Object context)
        {
            if (context == _projectContext)
            {
                _projectContext = null;
                _initialized = false;
            }
        }
    }
}