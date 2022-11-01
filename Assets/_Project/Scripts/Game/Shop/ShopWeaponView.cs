using Project.Application;
using Project.Game.Weapons;

namespace Project.Game.Shop
{
    public class ShopWeaponView : ShopInventoryItemView
    {
        protected override void OnInitialize(BaseData baseData)
        {
            var weaponData = baseData as WeaponData;
        }
    }
}