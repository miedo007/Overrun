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

        protected override void OnInjectStart()
        {
            UnityEngine.Application.targetFrameRate = 60;

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

            // In-progress session
            var inProgressSession = new InProgressSessionInfo();
            var inProgressReadWriter = new FileReadWriter("in-progress");
            saveManager.TryLoad(inProgressSession, success =>
            {
                if (!success) inProgressSession.Create();
            }, inProgressReadWriter);
            Bind(inProgressSession);

            // Player info
            var playerInfo = new PlayerInfo();
            var playerReadWriter = new FileReadWriter("player");
            saveManager.TryLoad(playerInfo, success =>
            {
                if (!success) playerInfo.Create();
            }, playerReadWriter);

            Bind(new SessionInfo
            {
                LevelIndex = playerInfo.PlayerSave.TopStageIndex
            });

            // Heroes registry
            var heroesInfo = new HeroRegistry();
            var heroesReadWriter = new FileReadWriter("heroes");
            saveManager.TryLoad(heroesInfo, _ =>
            {
                heroesInfo.Initialize(heroDatabase);
            }, heroesReadWriter);
            Bind(heroesInfo);

            // Finally inject the player info (and bind it)
            InjectAndBind(playerInfo);
        }
    }
}
