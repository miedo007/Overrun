using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Project.Game.UI
{
    public class CurrencyRewardView : MonoBehaviour
    {
        [SerializeField] private RectTransform root;
        [SerializeField] private TextMeshProUGUI valueField;

        public IEnumerator ShowRoutine(int value)
        {
            root.localScale = Vector3.zero;
            root.gameObject.SetActive(true);
            yield return root.DOScale(1, 0.125f);
        }

        public IEnumerator HideRoutine()
        {
            yield return root.DOScale(0, 0.125f);
            root.gameObject.SetActive(false);
        }

        public void SetValue(int currencyReward)
        {
            valueField.text = $"<sprite name=currency_gold> {currencyReward}";
        }
    }
}