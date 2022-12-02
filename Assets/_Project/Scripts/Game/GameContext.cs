using Project.PopupText;
using Project.Application;
using Project.Game.Cameras;
using Project.Game.Collectibles;
using Project.Game.Enemies;
using Project.Game.Items;
using Project.Game.Levels;
using Project.Game.Player;
using Project.Game.Targets;
using UnityEngine;

namespace Project.Game
{
    public class GameContext : SceneContext
    {
        [field: SerializeField] public PlayerInput PlayerInput { get; private set; }
        [field: SerializeField] public TargetManager TargetManager { get; private set; }
        [field: SerializeField] public PopupTextManager PopupTextManager { get; private set; }
        [field: SerializeField] public PlayerController PlayerController { get; private set; }
        [field: SerializeField] public LevelController LevelController { get; private set; }
        [field: SerializeField] public EnemyManager EnemyManager { get; private set; }
        [field: SerializeField] public CollectiblesManager CollectiblesManager { get; private set; }
        [field: SerializeField] public CameraManager CameraManager { get; private set; }
        [field: SerializeField] public ItemBehaviourManager ItemBehaviourManager { get; private set; }

        protected override void OnInjectStart()
        {
            base.OnInjectStart();
            
            Bind(PlayerInput);
            Bind(TargetManager);
            Bind(PopupTextManager);
            Bind(PlayerController);
            Bind(LevelController);
            Bind(EnemyManager);
            Bind(CollectiblesManager);
            Bind(CameraManager);
            Bind(ItemBehaviourManager);
        }
    }
}