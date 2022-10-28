using UnityEngine;

namespace Project.Game.Weapons
{
    public interface IDamageReceiver
    {
        public bool ReceiveDamage(float damage);
    }
}