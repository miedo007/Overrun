using Project.Application;
using UnityEngine;

namespace Project.Game.Tutorials
{
    public class WeaponSlotsFullTutorialScreen : TutorialScreen
    {
        protected override void OnClosed()
                {
                    PrefKeys.SetCompletedWeaponSlotsFullTutorial(true);
                }
    }
}