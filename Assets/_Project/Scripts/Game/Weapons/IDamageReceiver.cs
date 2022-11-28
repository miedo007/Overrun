using UnityEngine;

namespace Project.Game.Weapons
{
    public interface IDamageReceiver
    {
        public bool ReceiveDamage(float damage, bool isCritical, Vector2 direction, float force, GameObject sender);
    }
}