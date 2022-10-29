using System;
using Mtl.Injection;
using Project.Game.Weapons;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerDamageController : MonoBehaviour, IDamageReceiver
    {
        [Inject] private readonly PlayerHealthController _healthController;
        
        
        public bool ReceiveDamage(float damage)
        {
            _healthController.ReduceHealth(damage);
            return true;
        }
    }
}