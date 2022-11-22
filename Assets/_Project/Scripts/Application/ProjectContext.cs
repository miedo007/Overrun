using Mtl.Injection;
using Mtl.Save;
using Project.Game;
using Project.Heroes;
using Project.Tiers;
using UnityEngine;

namespace Project.Application
{
    public class ProjectContext : InjectContext
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private TieredGroupDatabase weaponDatabase;
        [SerializeField] private TieredGroupDatabase itemDatabase;
        [SerializeField] private HeroDatabase heroDatabase;
        [SerializeField] private GameData gameData;
        [SerializeField] private SaveManager saveManager;
        
        protected override void OnInjectStart()
        {
            UnityEngine.Application.targetFrameRate = 60;
            Bind(sceneLoader);
            Bind(weaponDatabase, "weapons");
            Bind(itemDatabase, "items");
            Bind(heroDatabase);
            Bind(gameData);         
            Bind(saveManager);
            
            var playerInfo = new PlayerInfo();
            var playerReadWriter = new FileReadWriter("player");
            saveManager.TryLoad(playerInfo, (success) =>
            {
                if (!success)
                {
                    playerInfo.Create();
                }
            }, playerReadWriter);
            
            Bind(new SessionInfo
            {
                LevelIndex = playerInfo.PlayerSave.TopStageIndex
            });

            var heroesInfo = new HeroRegistry();
            var heroesReadWriter = new FileReadWriter("heroes");
            saveManager.TryLoad(heroesInfo, success =>
            {
                heroesInfo.Initialize(heroDatabase);
            }, heroesReadWriter);     
            Bind(heroesInfo);
            
            InjectAndBind(playerInfo);
        }
        
        protected override void OnPostSetup()
        {
        }
    }
}