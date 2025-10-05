using Mtl.Injection;
using Mtl.Save;
using Project.Game;
using Project.Heroes;
using Project.Tiers;
using Project.Feedback;
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
                _heroesInfo.Initialize(heroDatabase);
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
    }
}
