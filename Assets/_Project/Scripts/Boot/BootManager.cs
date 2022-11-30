using Mtl.Bonfire;
using Mtl.Injection;
using Mtl.Toolbox;
using Project.Application;
using UnityEngine;

namespace Project.Boot
{
    public class BootManager : MonoBehaviour, IInjectionReady
    {
        [Inject] private readonly IBonfire _bonfire;
        [Inject] private readonly SceneLoader _sceneLoader;

        public void OnReady()
        {
            _bonfire.Initialize(() => UnityThreading.ExecuteOnMainThread(LoadMainMenu));
        }

        private void LoadMainMenu() => _sceneLoader.LoadScene("main_menu");
    }
}