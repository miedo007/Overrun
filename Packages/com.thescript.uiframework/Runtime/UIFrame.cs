using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mtl.UiFramework
{
    [PublicAPI]
    public class UIFrame
    {
        public event Action<UIScreen> OnOpened;
        public event Action<UIScreen> OnClosed;

        private readonly Dictionary<Type, UIScreen> _createdScreens = new Dictionary<Type, UIScreen>();
        private readonly Dictionary<Type, UIScreenConfigData> _registeredScreens = new Dictionary<Type, UIScreenConfigData>();
        private Canvas _canvasRoot;
        private readonly List<UIScreen> _activeScreens = new List<UIScreen>();

        private const string CanvasRootName = "[UIFrame]";

        public void Initialize(UISettingsData settingsData, Camera camera = null)
        {
            if (!settingsData.CanvasRoot)
            {
                throw new Exception("A Canvas root needs to be provided in data");
            }

            _canvasRoot = Object.Instantiate(settingsData.CanvasRoot);
            _canvasRoot.name = CanvasRootName;

            if (camera != null)
            {
                _canvasRoot.renderMode = RenderMode.ScreenSpaceCamera;
                _canvasRoot.worldCamera = camera;
                _canvasRoot.planeDistance = 1;
            }

            foreach (var settingsDataScreen in settingsData.Screens)
            {
                var type = settingsDataScreen.UIScreen.GetType();
                _registeredScreens.Add(type, settingsDataScreen);
                if (!settingsDataScreen.CreateOnOpen)
                {
                    CreateScreen(settingsDataScreen.UIScreen);
                }
            }
        }

        public T Open<T>() where T : UIScreen
        {
            var screen = GetOrCreateScreen<T>();
            screen.Open();
            return screen;
        }

        public T Open<T, TU>(TU parameters) where T : UIScreen<TU>
        {
            var screen = GetOrCreateScreen<T>();
            screen.Open(parameters);
            return screen;
        }

        public T Get<T>() where T : UIScreen
        {
            var type = typeof(T);
            if (!_createdScreens.TryGetValue(type, out var screen))
            {
                throw new Exception($"UIScreen {type} is not created");
            }

            return (T)screen;
        }

        public void Close<T>() where T : UIScreen
        {
            Close(typeof(T));
        }

        public void Close(Type type)
        {
            if (!_createdScreens.TryGetValue(type, out var screenInstance))
            {
                throw new Exception($"UIScreen {type} is not registered");
            }

            screenInstance.Close();
        }
        
        private T GetOrCreateScreen<T>() where T : UIScreen
        {
            var type = typeof(T);
            if (_createdScreens.TryGetValue(type, out var screen)) return (T)screen;
            if (!_registeredScreens.TryGetValue(type, out var settingsData))
            {
                throw new Exception($"UIScreen {type} is not registered");
            }

            screen = CreateScreen(settingsData.UIScreen);

            return (T)screen;
        }

        private UIScreen CreateScreen(UIScreen uiScreen)
        {
            var screen = Object.Instantiate(uiScreen, _canvasRoot.transform, false);
            screen.OnOpenEvent += OnOpenEvent;
            screen.OnCloseEvent += OnCloseEvent;
            screen.gameObject.SetActive(false);
            _createdScreens.Add(uiScreen.GetType(), screen);
            screen.Create();
            return screen;
        }

        private void OnOpenEvent(UIScreen screen)
        {
            OnOpened?.Invoke(screen);
        }

        private void OnCloseEvent(UIScreen screen)
        {
            var type = screen.GetType();
            var screenConfigData = _registeredScreens[type];
            if (screenConfigData.DestroyOnClose)
            {
                Object.Destroy(screen.gameObject);
                _createdScreens.Remove(type);
            }

            OnClosed?.Invoke(screen);
        }
    }
}