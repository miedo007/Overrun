using Mtl.Injection;
using Project.Heroes;
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
            Bind(new HeroRegistry());
        }

        protected override void OnPostSetup()
        {
        }
    }
}