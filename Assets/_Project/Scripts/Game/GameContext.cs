using Project.Application;
using UnityEngine;

namespace Project.Game
{
    public class GameContext : SceneContext
    {
        [field: SerializeField] public UltimateJoystick Joystick { get; private set; }
        
        protected override void OnInjectStart()
        {
            base.OnInjectStart();
            Bind(Joystick);
        }
    }
}