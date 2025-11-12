using Mtl.Injection;
using Mtl.Save;
using Project.Game;
using Project.Heroes;
using Project.Tiers;
using Project.Feedback;
using Project.Game.Levels;
using UnityEngine;
using System.Collections;

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
        [SerializeField] private FeedbackController feedbackController;

        private InProgressSessionInfo _inProgressSession;
        private PlayerInfo _playerInfo;
        private HeroRegistry _heroesInfo;

        protected override void OnInjectStart()
        {
            UnityEngine.Application.targetFrameRate = 60;

            // Initialize Crazy Games SDK integration
            SaveSystemIntegration.Initialize();
            SaveSystemIntegration.OnLoginStatusChanged += OnUserLoginStatusChanged;

            // Bind core services and databases
            Bind(levelDatabase);
            Bind(sceneLoader);
            Bind(tierDatabase);
            Bind(weaponDatabase, "weapons");
            Bind(itemDatabase, "items");
            Bind(statUpgradeDatabase, "stat_upgrades");
            Bind(heroDatabase);
            Bind(gameData);
            Bind(saveManager);
            Bind(feedbackController);

            // Initialize save objects immediately (dependency injection requires this)
            InitializeSaveObjects();
            
            // Wait for SDK to become ready and reload data from correct storage
            StartCoroutine(WaitForSDKAndReloadData());
        }

        private void InitializeSaveObjects()
        {
            // In-progress session
            _inProgressSession = new InProgressSessionInfo();
            var inProgressReadWriter = SaveSystemIntegration.CreateOptimalReadWriter("in-progress");
            saveManager.TryLoad(_inProgressSession, success =>
            {
                if (!success) _inProgressSession.Create();
            }, inProgressReadWriter);
            Bind(_inProgressSession);

            // Player info
            _playerInfo = new PlayerInfo();
            _playerInfo.Initialize(saveManager);
            var playerReadWriter = SaveSystemIntegration.CreateOptimalReadWriter("player");
            saveManager.TryLoad(_playerInfo, success =>
            {
                if (!success) _playerInfo.Create();
            }, playerReadWriter);

            Bind(new SessionInfo
            {
                LevelIndex = _playerInfo.PlayerSave.TopStageIndex
            });

            // Heroes registry
            _heroesInfo = new HeroRegistry();
            var heroesReadWriter = SaveSystemIntegration.CreateOptimalReadWriter("heroes");
            saveManager.TryLoad(_heroesInfo, _ =>
            {
                _heroesInfo.Initialize(saveManager, heroDatabase);
            }, heroesReadWriter);
            Bind(_heroesInfo);

            // Finally inject the player info (and bind it)
            InjectAndBind(_playerInfo);
        }

        private void OnUserLoginStatusChanged(bool isLoggedIn)
        {
            Debug.Log($"User login status changed to: {(isLoggedIn ? "Logged in" : "Logged out")}");
            
            if (isLoggedIn)
            {
                // User just logged in - migrate existing save data
                MigrateExistingSaveData();
            }
        }

        private void MigrateExistingSaveData()
        {
            Debug.Log("Migrating existing save data to Crazy Games Data module");
            
            SaveSystemIntegration.MigrateFileDataToCrazyGamesData("player");
            SaveSystemIntegration.MigrateFileDataToCrazyGamesData("in-progress");
            SaveSystemIntegration.MigrateFileDataToCrazyGamesData("heroes");
        }

        private void UpdateSaveManagers()
        {
            // Save current state before switching
            saveManager.Save();
            
            // Note: For a complete implementation, you would need to modify SaveManager
            // to support changing ReadWriters at runtime. For now, this logs the intent.
            Debug.Log("Save managers should be updated to use Crazy Games Data module");
            
            // In a full implementation, you might want to:
            // 1. Clear current saves from SaveManager
            // 2. Re-register with new ReadWriters pointing to Crazy Games Data
            // 3. Reload the data from Crazy Games Data module
        }

        private void OnDestroy()
        {
            SaveSystemIntegration.OnLoginStatusChanged -= OnUserLoginStatusChanged;
        }
        
        private System.Collections.IEnumerator WaitForSDKAndReloadData()
        {
            Debug.Log("[ProjectContext] Waiting for CrazySDK to be ready...");
            
            // Wait for CrazySDK to be ready
            float waitTime = 0f;
            while (!SaveSystemIntegration.IsCrazySDKReady())
            {
                yield return new WaitForSeconds(0.5f);
                waitTime += 0.5f;
                if (waitTime >= 10f)
                {
                    Debug.LogWarning("[ProjectContext] CrazySDK not ready after 10 seconds, proceeding anyway");
                    yield break;
                }
            }
            
            Debug.Log($"[ProjectContext] CrazySDK is ready after {waitTime}s! Reloading save data from CrazyGames storage...");
            
            // Now that SDK is ready, reload all save data from the correct storage
            // The DynamicReadWriter will now use CrazyGames storage instead of local files
            ReloadAllSaveData();
        }
        
        private void ReloadAllSaveData()
        {
            Debug.Log("[ProjectContext] ReloadAllSaveData started");
            
            // Reload in-progress session
            var inProgressReadWriter = SaveSystemIntegration.CreateOptimalReadWriter("in-progress");
            Debug.Log($"[ProjectContext] Attempting to reload in-progress data...");
            if (inProgressReadWriter.TryLoad(out var rawInProgressData))
            {
                Debug.Log($"[ProjectContext] Raw in-progress data loaded: {rawInProgressData}");
                var inProgressData = Newtonsoft.Json.JsonConvert.DeserializeObject<InProgressSessionSave>(rawInProgressData);
                var inProgressSave = (InProgressSessionSave)_inProgressSession.Save;
                inProgressSave.InProgress = inProgressData.InProgress;
                inProgressSave.LevelIndex = inProgressData.LevelIndex;
                inProgressSave.WaveIndex = inProgressData.WaveIndex;
                Debug.Log($"[ProjectContext] In-progress data reloaded - InProgress: {inProgressSave.InProgress}, Level: {inProgressSave.LevelIndex}, Wave: {inProgressSave.WaveIndex}");
            }
            else
            {
                Debug.Log("[ProjectContext] No in-progress data found in CrazyGames storage");
            }
            
            // Reload player data
            var playerReadWriter = SaveSystemIntegration.CreateOptimalReadWriter("player");
            Debug.Log($"[ProjectContext] Attempting to reload player data...");
            if (playerReadWriter.TryLoad(out var rawPlayerData))
            {
                Debug.Log($"[ProjectContext] Raw player data loaded: {rawPlayerData}");
                var playerData = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerSave>(rawPlayerData);
                var playerSave = (PlayerSave)_playerInfo.Save;
                
                Debug.Log($"[ProjectContext] BEFORE reload - TopStageIndex: {playerSave.TopStageIndex}, Currency: {playerSave.Currency}");
                
                playerSave.Currency = playerData.Currency;
                playerSave.TopStageIndex = playerData.TopStageIndex;
                playerSave.HasUsedHeroAdUpgradeThisSession = playerData.HasUsedHeroAdUpgradeThisSession;
                playerSave.LastHeroAdUpgradeTime = playerData.LastHeroAdUpgradeTime;
                
                Debug.Log($"[ProjectContext] AFTER reload - TopStageIndex: {playerSave.TopStageIndex}, Currency: {playerSave.Currency}");
                
                _playerInfo.NotifyDataReloaded();
                Debug.Log($"[ProjectContext] Player data reloaded successfully!");
            }
            else
            {
                Debug.Log("[ProjectContext] No player data found in CrazyGames storage");
            }
            
            // Reload hero data
            var heroReadWriter = SaveSystemIntegration.CreateOptimalReadWriter("heroes");
            Debug.Log($"[ProjectContext] Attempting to reload hero data...");
            if (heroReadWriter.TryLoad(out var rawHeroData))
            {
                Debug.Log($"[ProjectContext] Raw hero data loaded: {rawHeroData}");
                var heroSaveData = Newtonsoft.Json.JsonConvert.DeserializeObject<Project.Heroes.HeroSave>(rawHeroData);
                var registrySave = (Project.Heroes.HeroSave)_heroesInfo.Save;
                registrySave.SelectedHero = heroSaveData.SelectedHero;
                registrySave.HeroLevels = heroSaveData.HeroLevels;
                _heroesInfo.SetActiveHero(_heroesInfo.GetSelectedHero());
                Debug.Log($"[ProjectContext] Hero data reloaded - Selected: {registrySave.SelectedHero}");
            }
            else
            {
                Debug.Log("[ProjectContext] No hero data found in CrazyGames storage");
            }
            
            Debug.Log("[ProjectContext] ReloadAllSaveData completed");
        }
    }
}
