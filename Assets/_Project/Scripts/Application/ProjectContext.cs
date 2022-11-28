using System.Threading;
using Mtl.Injection;
using Mtl.Save;
using Project.Game;
using Project.Heroes;
using Project.Tiers;
using Mtl.Bonfire;
using UnityEngine;

namespace Project.Application
{
    public class ProjectContext : InjectContext
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private TierDatabase tierDatabase;
        [SerializeField] private TieredGroupDatabase weaponDatabase;
        [SerializeField] private TieredGroupDatabase itemDatabase;
        [SerializeField] private HeroDatabase heroDatabase;
        [SerializeField] private GameData gameData;
        [SerializeField] private SaveManager saveManager;
        
        protected override void OnInjectStart()
        {
            UnityEngine.Application.targetFrameRate = 60;
            
            var beaconWrapper = GetComponentInChildren<BeaconWrapper>();
            Bind(beaconWrapper);

            var analyticsManager = new AnalyticsManager();
            Bind<IAnalyticsProvider>(analyticsManager);
            analyticsManager.Register(beaconWrapper);
            analyticsManager.Register(new GameAnalyticsWrapper());
            
            Bind(sceneLoader);
            Bind(tierDatabase);
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
            base.OnPostSetup();
            InitBonfire();
        }

        private async void InitBonfire()
        {
            var sem = new SemaphoreSlim(0, 1);
            var beaconWrapper = Get<BeaconWrapper>();

            var bonfire = (IBonfire)beaconWrapper;
            bonfire.Initialize(() => sem.Release());
            await sem.WaitAsync();

            const string gameWasLaunched = "GameWasLaunched";
            if (PlayerPrefs.GetInt(gameWasLaunched, 0) == 1)
            {
                var termsOfService = (ITermsOfService)beaconWrapper;
                termsOfService.Show(_ => sem.Release());
                await sem.WaitAsync();
            }
            else
            {
                PlayerPrefs.SetInt(gameWasLaunched, 1);
                PlayerPrefs.Save();
            }
        }
    }
}