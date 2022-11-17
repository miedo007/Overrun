using System;
using Mtl.Injection;
using Project.Application;
using Project.Game.Weapons;
using Project.Heroes;
using Project.Stats;
using TMPro;
using UnityEngine;

namespace Project.Game.Shop
{
    public class InfoViewWeapon : InfoViewBase
    {
        [SerializeField] private TextMeshProUGUI rangeText;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI criticalDamageText;
        [SerializeField] private TextMeshProUGUI cooldownText;
        [SerializeField] private TextMeshProUGUI knockbackText;

        private HeroInfo _heroInfo;
        private WeaponData _weaponData;


        private void OnStatChanged(StatInfo obj)
        {
            Refresh();
        }

        public override void Initialize(BaseData data)
        {
            _heroInfo = InjectionContainer.Instance.Injector.Get<HeroRegistry>().ActiveHero;
            
            foreach (var stat in _heroInfo.Stats)
            {
                stat.Changed += OnStatChanged;
            }
            
            gameObject.name = data.name;
            _weaponData = data as WeaponData;
            Refresh();
        }
        
        private void OnDestroy()
        {
            if (_heroInfo == null)
            {
                return;
            }

            foreach (var stat in _heroInfo.Stats)
            {
                stat.Changed -= OnStatChanged;
            }
        }

        private void Refresh()
        {
            rangeText.text = _weaponData.GetRangeDisplayText(_heroInfo);
            damageText.text = _weaponData.GetDamageDisplayText(_heroInfo);
            criticalDamageText.text = _weaponData.GetCritDamageDisplayText(_heroInfo);
            cooldownText.text = _weaponData.GetCooldownDisplayText(_heroInfo);
            knockbackText.text = _weaponData.GetKnockbackDisplayText(_heroInfo);
        }
    }
}