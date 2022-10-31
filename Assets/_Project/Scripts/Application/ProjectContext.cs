using Mtl.Injection;
using Project.Game.Weapons;
using UnityEngine;

namespace Project.Application
{
    public class ProjectContext : InjectContext
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private WeaponDatabase weaponDatabase;
        
        protected override void OnInjectStart()
        {
            UnityEngine.Application.targetFrameRate = 60;
            Bind(sceneLoader);
            Bind(weaponDatabase);
        }

        protected override void OnPostSetup()
        {
        }
    }
}