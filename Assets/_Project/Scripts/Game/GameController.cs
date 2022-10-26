using Mtl.Injection;
using Mtl.UiFramework;
using UnityEngine;

namespace Project.Game
{
    public class GameController : MonoBehaviour
    {
        [Inject] private readonly UIFrame _uiFrame;

    }
}