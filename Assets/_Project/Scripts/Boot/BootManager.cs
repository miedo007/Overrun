using Mtl.Injection;
using Mtl.Toolbox;
using Project.Application;
using UnityEngine;

namespace Project.Boot
{
    public class BootManager : MonoBehaviour, IInjectionReady
    {
        [Inject] private readonly SceneLoader _sceneLoader;

        public void OnReady()
        {
            // Previously initialized Beacon/Bonfire, then queued main menu load.
            // Now just queue the main menu load on the main thread.
            UnityThreading.ExecuteOnMainThread(LoadMainMenu);
        }

        private void LoadMainMenu() => _sceneLoader.LoadScene("main_menu");
    }
}
