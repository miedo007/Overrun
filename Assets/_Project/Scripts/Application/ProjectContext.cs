using Mtl.Injection;
using Project.Game;
using Project.Game.Items;
using Project.Game.Weapons;
using Project.Tiers;
using UnityEngine;

namespace Project.Application
{
    public class ProjectContext : InjectContext
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private TieredGroupDatabase weaponDatabase;
        [SerializeField] private TieredGroupDatabase itemDatabase;
        [SerializeField] private GameData gameData;
        
        protected override void OnInjectStart()
        {
            UnityEngine.Application.targetFrameRate = 60;
            Bind(sceneLoader);
            Bind(weaponDatabase, "weapons");
            Bind(itemDatabase, "items");
            Bind(gameData);
        }

        protected override void OnPostSetup()
        {
        }
    }
}