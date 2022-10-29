using Mtl.UiFramework;
using UnityEngine;

namespace Project.Game.UI
{
    public class HudScreen : UIScreen
    {
        [field: SerializeField] public PlayerHealthMeter PlayerHealthMeter { get; private set; }
    }
}