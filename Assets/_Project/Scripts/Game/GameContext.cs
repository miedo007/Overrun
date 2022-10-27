using Project.PopupText;
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
        [field: SerializeField] public PopupTextManager PopupTextManager { get; private set; }
                
        protected override void OnInjectStart()
        {
            base.OnInjectStart();
            
            Bind(Joystick);
            Bind(TargetManager);
            Bind(PopupTextManager);

            var heroRegistry = new HeroRegistry();
            var heroInfo = heroRegistry.GetActiveHeroInfo();
            Bind(heroInfo);
        }
    }
}