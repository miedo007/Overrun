using Mtl.Injection;
using Project.Game;
using Project.Game.Items;
using Project.Game.Weapons;
using UnityEngine;

namespace Project.Application
{
    public class ProjectContext : InjectContext
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private WeaponDatabase weaponDatabase;
        [SerializeField] private ItemDatabase itemDatabase;
        [SerializeField] private GameData gameData;
        
        protected override void OnInjectStart()
        {
            UnityEngine.Application.targetFrameRate = 60;
            Bind(sceneLoader);
            Bind(weaponDatabase);
            Bind(itemDatabase);
            Bind(gameData);
        }

        protected override void OnPostSetup()
        {
        }
    }
}