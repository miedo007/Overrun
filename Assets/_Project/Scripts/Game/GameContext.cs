using Mtl.Injection;
using Project.Application;
using Project.Game.Targets;
using Project.Heroes;
using UnityEngine;

namespace Project.Game
{
    public class GameContext : SceneContext
    {
        [field: SerializeField] public UltimateJoystick Joystick { get; private set; }
        [field: SerializeField] public TargetManager TargetManager { get; private set; }

        [Inject] private readonly HeroRegistry _heroRegistry;
                
        protected override void OnInjectStart()
        {
            base.OnInjectStart();
            Bind(Joystick);
            Bind(TargetManager);

            var heroInfo = _heroRegistry.GetActiveHeroInfo();
            Bind(heroInfo);
        }
    }
}