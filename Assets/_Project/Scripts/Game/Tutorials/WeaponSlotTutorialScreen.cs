using Project.Application;

namespace Project.Game.Tutorials
{
    public class WeaponSlotTutorialScreen : TutorialScreen
    {
        protected override void OnClosed()
        {
            PrefKeys.SetCompletedWeaponSlotTutorial(true);
        }
    }
}