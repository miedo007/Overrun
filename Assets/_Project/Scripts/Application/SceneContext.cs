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
            uiFrame.Initialize(uiSettings, Camera.main);

            // TODO Update ui framework package to allow setting of sorting layer in UI settings data
            var canvas = GameObject.Find("[UIFrame]").GetComponent<Canvas>();
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 1;
            //
            
            Bind(uiFrame);
        }
    }
}