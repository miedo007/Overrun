using Mtl.Bonfire;
using Mtl.Injection;
using Mtl.Toolbox;
using Project.Application;
using UnityEngine;

namespace Project.Boot
{
    public class BootManager : MonoBehaviour, IInjectionReady
    {
        [Inject] private readonly BeaconWrapper _beaconWrapper;
        [Inject] private readonly SceneLoader _sceneLoader;

        public void OnReady()
        {
            var bonfire = (IBonfire)_beaconWrapper;
            bonfire.Initialize(() => UnityThreading.ExecuteOnMainThread(LoadMainMenu));
        }

        private void LoadMainMenu() => _sceneLoader.LoadScene("main_menu");
    }
}