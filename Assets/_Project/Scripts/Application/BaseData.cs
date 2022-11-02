using Project.Heroes;
using UnityEngine;

namespace Project.Application
{
    public class BaseData : ScriptableObject
    {
        [field: SerializeField] public Sprite Sprite { get; protected set; }
        [field: SerializeField] public string DisplayName { get; protected set; }
        [field: SerializeField, TextArea] public string Description { get; protected set; }
        [field: SerializeField] public int BasePrice { get; set; } = 5;

        public virtual string GetDescriptionForHero(HeroInfo heroInfo)
        {
            return Description;
        }
    }
}