using Project.Application;

namespace Project.Game.Tutorials
{
    public class WeaponMergeTutorialScreen : TutorialScreen
    {
        protected override void OnClosed()
        {
            base.OnClosed();
            PrefKeys.SetCompletedWeaponMergeTutorial(true);
        }
    }
}