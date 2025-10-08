using Project.Heroes;
using Project.Tiers;
using UnityEngine;

namespace Project.Application
{
    public class BaseData : ScriptableObject
    {
        [field: SerializeField] public TierData Tier { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; protected set; }
        [field: SerializeField] public string DisplayName { get; protected set; }
        [field: SerializeField, TextArea] public string Description { get; protected set; }
        [field: SerializeField] public int BasePrice { get; set; } = 5;
        [field: SerializeField] public bool IsAdExclusive { get; protected set; } = false;

        public string Id => name;
        
        public virtual string GetDescriptionForHero(HeroInfo heroInfo)
        {
            return Description;
        }

        public string GetDisplayStringForValue(float value, bool displayAsPercentage)
        {
            var postfix = displayAsPercentage ? "%" : "";
            value = displayAsPercentage ? value * 100f : value;
            
            if (value - Mathf.Round(value) == 0)
            {
                return $"{value:0}{postfix}";
            }
            else
            {
                return $"{value:0.0}{postfix}";
            }
        }
    }
}