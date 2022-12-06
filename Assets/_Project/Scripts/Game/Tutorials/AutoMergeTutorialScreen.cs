using Project.Application;
using UnityEngine;

namespace Project.Game.Tutorials
{
    public class AutoMergeTutorialScreen : TutorialScreen
    {
        protected override void OnClosed()
        {
            base.OnClosed();
            PrefKeys.SetCompletedAutoMergeTutorial(true);
        }
    }
}