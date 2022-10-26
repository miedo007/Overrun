using Mtl.Injection;
using UnityEngine;

namespace Project.Application
{
    public class ProjectContext : InjectContext
    {
        [SerializeField] private SceneLoader sceneLoader;
        
        protected override void OnInjectStart()
        {
            UnityEngine.Application.targetFrameRate = 60;
            Bind(sceneLoader);
        }

        protected override void OnPostSetup()
        {
        }
    }
}