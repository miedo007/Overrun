using Mtl.Injection;
using Mtl.UiFramework;
using UnityEngine;

namespace Project.Application
{
    public class SceneContext : InjectContext
    {
        [SerializeField] private UISettingsData uiSettings;

        protected override void OnInjectStart()
        {
            CreateAndBindUiFrame();
        }

        private void CreateAndBindUiFrame()
        {
            var uiFrame = new UIFrame();
            uiFrame.Initialize(uiSettings);
            Bind(uiFrame);
        }
    }
}