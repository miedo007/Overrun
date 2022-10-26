using System;
using System.Collections;
using UnityEngine;

namespace Project.Game.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        public event Action Activated;
        
        public WeaponData Data { get; private set; }
        
        private float _lastActivationTime;
        
        public void Initialize(WeaponData weaponData)
        {
            Data = weaponData;
        }
        
        public bool ShouldActivate(float time)
        {
            return time >= _lastActivationTime + Data.ActivationRate;
        }

        public void Activate(float time)
        {
            _lastActivationTime = time;
            StartCoroutine(ActivationRoutine());
        }

        private IEnumerator ActivationRoutine()
        {
            yield return Data.BehaviourBase.ActivationRoutine();
        }
    }
}