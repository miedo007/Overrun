using Mtl.Injection;
using Mtl.UiFramework;
using Project.Game.UI;
using UnityEngine;

namespace Project.Game
{
    public class GameController : MonoBehaviour
    {
        [Inject] private readonly UIFrame _uiFrame;

        private void Start()
        {
            _uiFrame.Open<HudScreen>();
        }
    }
}