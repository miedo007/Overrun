using Project.Application;

namespace Project.Game.Tutorials
{
    public class CraftingTutorialScreen : TutorialScreen
    {
        protected override void OnClosed()
        {
            base.OnClosed();
            PrefKeys.SetCompletedCraftingTutorial(true);
        }
    }
}