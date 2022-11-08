using System;
using Mtl.Injection;
using Project.Game.Weapons;
using Project.PopupText;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerDamageController : MonoBehaviour, IDamageReceiver
    {
        [field: SerializeField] public PopupTextItem DamageTextPrefab { get; private set; }
        [field: SerializeField] public Color DamageTextColor { get; private set; }
        
        [Inject] private readonly PlayerHealthController _healthController;
        [Inject] private readonly PopupTextManager _popupTextManager;
        
        
        public bool ReceiveDamage(float damage, bool isCritical, Vector2 force, GameObject sender)
        {
            _healthController.ReduceHealth(damage);
            _popupTextManager.DisplayTextAtPosition($"{damage:0.#}", DamageTextColor, transform.position, DamageTextPrefab);
            return true;
        }
    }
}