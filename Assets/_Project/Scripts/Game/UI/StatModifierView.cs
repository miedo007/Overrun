using Project.Stats;
using TMPro;
using UnityEngine;

namespace Project.Game.UI
{
    public class StatModifierView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI valueText;

        public void Initialize(StatModifier statModifier)
        {
            valueText.text = statModifier.GetDisplayValue();
            gameObject.SetActive(true);
        }
    }
}