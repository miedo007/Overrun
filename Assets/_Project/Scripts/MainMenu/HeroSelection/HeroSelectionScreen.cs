using System;
using Mtl.UiFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Project.MainMenu.HeroSelection
{
    public class HeroSelectionScreen : UIScreen
    {
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            closeButton.onClick.AddListener(Close);
        }
    }
}