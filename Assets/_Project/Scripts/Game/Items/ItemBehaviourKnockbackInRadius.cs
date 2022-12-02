using System;
using Mtl.Injection;
using Project.Game.Enemies;
using Project.Game.Player;
using Project.Heroes;
using Project.Stats;
using UnityEngine;

namespace Project.Game.Items
{
    [CreateAssetMenu(fileName = "item_behaviour_enemy_knockback_", menuName = "Data/Items/Behaviours/EnemyKnockback", order = 0)]
    public class ItemBehaviourKnockbackInRadius : ItemBehaviour
    {
        [field: SerializeField] public StatData KnockbackStat { get; private set; }
        [field: SerializeField] public float KnockbackStrength { get; private set; } = 10f;
        [field: SerializeField] public float Radius { get; private set; } = 3;
        [field: SerializeField] public LayerMask LayerMask { get; private set; } = 3;
        
        [NonSerialized] private StatInfo _knockbackStatInfo;
        [NonSerialized] private PlayerController _playerController;

        private static readonly Collider2D[] Results = new Collider2D[32];
        
        public StatInfo KnockbackStatInfo
        {
            get
            {
                if (_knockbackStatInfo == null)
                {            
                    var heroInfo = InjectionContainer.Instance.Injector.Get<HeroRegistry>().ActiveHero;
                    _knockbackStatInfo = heroInfo.GetStat(KnockbackStat);
                }

                return _knockbackStatInfo;
            }
        }

        public PlayerController PlayerController
        {
            get
            {
                if (_playerController == null)
                {
                    _playerController = InjectionContainer.Instance.Injector.Get<PlayerController>();
                }

                return _playerController;
            }
        }

        public override bool OnPerform(Vector3 position)
        {
            var resultCount = Physics2D.OverlapCircleNonAlloc(position, Radius, Results, LayerMask);
            if (resultCount == 0)
            {
                return false;
            }
            
            var forceStrength = KnockbackStrength * KnockbackStatInfo.GetFloatValue();
            for (int i = 0; i < resultCount; i++)
            {
                var enemy = Results[i].GetComponent<EnemyController>();
                if (enemy == null)
                {
                    continue;
                }
                
                enemy.Knockback((enemy.Position - (Vector2)PlayerController.Position).normalized * forceStrength);
            }

            return true;
        }

        public override Vector3 GetFeedbackPosition(Vector3 defaultPosition)
        {
            return PlayerController.Position;
        }

        public override string GetDescription()
        {
            return $"{Chance * 100:0.0}% chance to knock back enemies within {Radius:0.0}m";
        }

        public override void Cleanup()
        {
            _knockbackStatInfo = null;
            _playerController = null;
        }
    }
}