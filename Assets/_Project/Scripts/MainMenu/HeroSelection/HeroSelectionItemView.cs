using System;
using Project.Heroes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.MainMenu.HeroSelection
{
    public class HeroSelectionItemView : MonoBehaviour, IPointerClickHandler
    {
        public event Action<HeroData> Selected;
        [SerializeField] private Image heroImage;
        [SerializeField] private TextMeshProUGUI heroNameText;

        private HeroData _heroData;
        
        public void Initialize(HeroData heroData)
        {
            _heroData = heroData;
            heroImage.sprite = heroData.Sprite;
            heroNameText.text = heroData.DisplayName;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Selected?.Invoke(_heroData);
        }
    }
}