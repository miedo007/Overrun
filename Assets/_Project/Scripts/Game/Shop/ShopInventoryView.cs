using System.Collections.Generic;
using Mtl.Injection;
using Project.Application;
using Project.Game.Items;
using Project.Game.Weapons;
using UnityEngine;

namespace Project.Game.Shop
{
    public class ShopInventoryView : MonoBehaviour
    {
        [field: SerializeField] public ShopInventoryRow WeaponRow { get; private set; }
        [field: SerializeField] public ShopInventoryRow ItemRow { get; private set; }
        
        [Inject] private readonly WeaponDatabase _weaponDatabase;
        [Inject] private readonly ItemDatabase _itemDatabase;

        public void Populate()
        {
            var weaponCount = WeaponRow.Slots.Length;
            var weaponDatas = new List<BaseData>();
            for (var i = 0; i < weaponCount; i++)
            {
                weaponDatas.Add(_weaponDatabase.GetRandom());
            }

            WeaponRow.Populate(weaponDatas);
            
            var itemCount = ItemRow.Slots.Length;
            var itemDatas = new List<BaseData>();
            for (var i = 0; i < itemCount; i++)
            {
                itemDatas.Add(_itemDatabase.GetRandom());
            }

            ItemRow.Populate(itemDatas);
        }
    }
}