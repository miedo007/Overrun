using Project.Heroes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.MainMenu.HeroSelection
{
    public class HeroSelectionItemView : MonoBehaviour
    {
        [SerializeField] private Image heroImage;
        [SerializeField] private TextMeshProUGUI heroNameText;

        public void Initialize(HeroData heroData)
        {
            heroImage.sprite = heroData.Sprite;
            heroNameText.text = heroData.DisplayName;
        }
    }
}