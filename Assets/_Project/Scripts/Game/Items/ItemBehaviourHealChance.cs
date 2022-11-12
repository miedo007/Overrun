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
        [SerializeField] private float chance = 0.01f;

        private PlayerHealthController _healthController;

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

        public override void Perform()
        {
            var healthController = HealthController;
            if (healthController.IsFull || Random.value > chance)
            {
                return;
            }
            
            if (isPercentage)
            {
                healthController.HealByPercentage(amount);
            }
            else
            {
                healthController.HealByAmount(amount);
            }
        }

        public override string GetDescription()
        {
            var amountDisplayText = isPercentage
                ? $"{amount * 100:0}% of <sprite tint=1 name={HealthController.HealthStat.Icon.name}>"
                : $"{amount:0.0}";
            return $"{chance * 100:0.0}% chance to heal {amountDisplayText}";
        }
    }
}