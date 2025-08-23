using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Mtl.UiFramework
{
    [PublicAPI]
    [CreateAssetMenu(fileName = "UISettingsData", menuName = "MTL/UI/UI Settings")]
    public class UISettingsData : ScriptableObject
    {
        [field: SerializeField] public Canvas CanvasRoot { get; private set; }
        [SerializeField] private UIScreenConfigData[] screens;

        public IEnumerable<UIScreenConfigData> Screens => screens;
    }

    [Serializable]
    public class UIScreenConfigData
    {
        [field: SerializeField] public UIScreen UIScreen { get; private set; }
        [field: SerializeField] public bool CreateOnOpen { get; private set; }
        [field: SerializeField] public bool DestroyOnClose { get; private set; }
    }
}