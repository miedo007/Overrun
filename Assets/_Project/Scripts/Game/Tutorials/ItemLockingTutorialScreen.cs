using Project.Application;

namespace Project.Game.Tutorials
{
    public class ItemLockingTutorialScreen : TutorialScreen
    {
        protected override void OnClosed()
        {
            base.OnClosed();
            PrefKeys.SetsCompletedItemLockingTutorial(true);
        }
    }
}