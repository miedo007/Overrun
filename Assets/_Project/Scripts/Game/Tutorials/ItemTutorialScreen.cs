using Project.Application;
using UnityEngine;

namespace Project.Game.Tutorials
{
    public class ItemTutorialScreen : TutorialScreen
    {
        protected override void OnClosed()
        {
            base.OnClosed();
            PrefKeys.SetCompletedItemsTutorial(true);
        }
    }
}