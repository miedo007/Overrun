using System.Threading;
using Mtl.Injection;
using Mtl.Save;
using Project.Game;
using Project.Heroes;
using Project.Tiers;
using Mtl.Bonfire;
using Project.Game.Levels;
using UnityEngine;

namespace Project.Application
{
    public class ProjectContext : InjectContext
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private LevelDatabase levelDatabase;
        [SerializeField] private TierDatabase tierDatabase;
        [SerializeField] private TieredGroupDatabase weaponDatabase;
        [SerializeField] private TieredGroupDatabase itemDatabase;
        [SerializeField] private TieredGroupDatabase statUpgradeDatabase;
        [SerializeField] private HeroDatabase heroDatabase;
        [SerializeField] private GameData gameData;
        [SerializeField] private SaveManager saveManager;
        
        protected override void OnInjectStart()
        {
            UnityEngine.Application.targetFrameRate = 60;
            
            var beaconWrapper = GetComponentInChildren<BeaconWrapper>();
            Bind<IBonfire>(beaconWrapper);

            var analyticsManager = new AnalyticsManager();
            Bind<IAnalyticsManager>(analyticsManager);
            analyticsManager.Register(beaconWrapper);
            analyticsManager.Register(new GameAnalyticsWrapper());
            
            Bind(levelDatabase);
            Bind(sceneLoader);
            Bind(tierDatabase);
            Bind(weaponDatabase, "weapons");
            Bind(itemDatabase, "items");
            Bind(statUpgradeDatabase, "stat_upgrades");
            Bind(heroDatabase);
            Bind(gameData);         
            Bind(saveManager);

            var inProgressSession = new InProgressSessionInfo();
            var inProgressReadWriter = new FileReadWriter("in-progress");
            saveManager.TryLoad(inProgressSession, (success) =>
            {
                if (!success)
                {
                    inProgressSession.Create();
                }
            }, inProgressReadWriter);
            Bind(inProgressSession);
            
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

    }
}