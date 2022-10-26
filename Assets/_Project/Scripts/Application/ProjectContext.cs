using Mtl.Injection;
using UnityEngine;

namespace Project.Application
{
    public class ProjectContext : InjectContext
    {
        [SerializeField] private SceneLoader sceneLoader;
        
        protected override void OnInjectStart()
        {
            Bind(sceneLoader);
        }

        protected override void OnPostSetup()
        {
        }
    }
}