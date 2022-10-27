using Project.Game.Player;
using Project.Stats;
using UnityEngine;

namespace Project.Heroes
{
    [CreateAssetMenu(fileName = "data_hero_", menuName = "Data/Heroes/HeroData", order = 0)]
    public class HeroData : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; }
        [field: SerializeField] public CharacterStats Stats { get; private set; }
        [field: SerializeField] public HeroViewController Prefab { get; private set; }
    }
}