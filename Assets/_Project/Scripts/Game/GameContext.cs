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
                
        protected override void OnInjectStart()
        {
            base.OnInjectStart();
            Bind(Joystick);
            Bind(TargetManager);

            var heroRegistry = new HeroRegistry();
            var heroInfo = heroRegistry.GetActiveHeroInfo();
            Bind(heroInfo);
        }
    }
}