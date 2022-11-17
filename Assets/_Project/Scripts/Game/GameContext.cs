using Mtl.Injection;
using Project.PopupText;
using Project.Application;
using Project.Game.Collectibles;
using Project.Game.Enemies;
using Project.Game.Levels;
using Project.Game.Player;
using Project.Game.Targets;
using Project.Heroes;
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
        [field: SerializeField] public HeroInfo HeroInfo { get; private set; }

        protected override void OnInjectStart()
        {
            base.OnInjectStart();

            var heroesInfo = InjectionContainer.Instance.Injector.Get<HeroesInfo>();
            HeroInfo = heroesInfo.GetSelectedHeroInfo();
            Bind(HeroInfo);
            
            Bind(PlayerInput);
            Bind(TargetManager);
            Bind(PopupTextManager);
            Bind(PlayerController);
            Bind(LevelController);
            Bind(EnemyManager);
            Bind(CollectiblesManager);
        }
    }
}