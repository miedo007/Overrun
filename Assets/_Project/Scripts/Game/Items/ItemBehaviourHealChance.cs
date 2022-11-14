using System;
using Mtl.Injection;
using Project.Game.Player;
using UnityEngine;

namespace Project.Game.Items
{
    [CreateAssetMenu(fileName = "item_behaviour_heal_chance_", menuName = "Data/Items/Behaviours/HealChance", order = 0)]
    public class ItemBehaviourHealChance : ItemBehaviour
    {
        [SerializeField] private float amount = 1f;
        [SerializeField] private bool isPercentage = false;

        [NonSerialized] private PlayerHealthController _healthController;

        public PlayerHealthController HealthController
        {
            get
            {
                if (_healthController == null)
                {            
                    _healthController = InjectionContainer.Instance.Injector.Get<PlayerHealthController>();
                }

                return _healthController;
            }
        }

        public override bool OnPerform(Vector3 position)
        {
            var healthController = HealthController;
            if (healthController.IsFull)
            {
                return false;
            }
            
            if (isPercentage)
            {
                healthController.HealByPercentage(amount);
            }
            else
            {
                healthController.HealByAmount(amount);
            }

            return true;
        }

        public override string GetDescription()
        {
            var amountDisplayText = isPercentage ? $"{amount * 100:0}% of <sprite tint=1 name={HealthController.HealthStat.Icon.name}>"
                : $"{amount:0.0}";
            return $"{GetChanceDisplay()} to heal {amountDisplayText}";
        }

        public override void Cleanup()
        {
            _healthController = null;
        }
    }
}