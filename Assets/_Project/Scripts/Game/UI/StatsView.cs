using System;
using Mtl.Injection;
using Project.Heroes;
using Project.Stats;
using TMPro;
using UnityEngine;

namespace Project.Game.UI
{
    public class StatsView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Color negativeColor = Color.red;
        [SerializeField] private Color positiveColor = Color.green;
        [SerializeField] private Color neutralColor = Color.white;
        
        
        [Inject] private HeroInfo _heroInfo;

        private void Start()
        {
            foreach (var stat in _heroInfo.Stats)
            {
                stat.Changed += OnStatChanged;
            }
            Refresh();
        }
        
        private void OnDestroy()
        {
            foreach (var stat in _heroInfo.Stats)
            {
                stat.Changed -= OnStatChanged;
            }
        }

        private void OnStatChanged(StatInfo obj)
        {
            Refresh();
        }

        public void Refresh()
        {
            string statsString = "";
            
            foreach (var stat in _heroInfo.Stats)
            {
                var color = neutralColor;
                var baseValue = stat.BaseValue;
                var value = stat.GetFloatValue();
                
                if (value < baseValue)
                {
                    color = negativeColor;
                }
                else if (value > baseValue)
                {
                    color = positiveColor;
                }

                var hexString = ColorUtility.ToHtmlStringRGBA(color);
                
                statsString += ( $"<color=#{hexString}><sprite name={stat.Data.Icon.name}> {value:0.0}</color>\n");
            }

            text.text = statsString;
        }
    }
}