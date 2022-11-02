using System.Collections.Generic;
using Project.Application;
using Project.Heroes;
using Project.Stats;
using UnityEngine;

namespace Project.Game.Items
{
    [CreateAssetMenu(fileName = "data_item_", menuName = "Data/Items/ItemData", order = 0)]
    public class ItemData : BaseData
    {
        [field: SerializeField] public List<StatModifier> StatModifiers { get; private set; }
        
        public override string GetDescriptionForHero(HeroInfo heroInfo)
        {
            var description = "";
            foreach (var statModifier in StatModifiers)
            {
                var color = "#42B708";
                var prefix = "+";
                var postfix = "";
                var value = statModifier.Value;
                if (value < 0)
                {
                    color = "#D91C36";
                    prefix = "-";
                }

                if (statModifier.ModifierType == StatModifierType.PercentAdd)
                {
                    value *= 100f;
                    postfix = "%";
                }

                description += ( $"<b><color={color}><sprite tint=1 name={statModifier.StatData.Icon.name}> {prefix}{Mathf.Abs(value):0.0}{postfix}</color></b>\n");
            }
            return description;
        }
    }
}