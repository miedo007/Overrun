using System;
using System.Collections;
using Project.Game.Targets;
using UnityEngine;

namespace Project.Game.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        public event Action Activated;
        
        [field: SerializeField] public Transform Barrel { get; set; }
        
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

        public void Activate(float time, WeaponController weapon)
        {
            _lastActivationTime = time;
            StartCoroutine(ActivationRoutine(weapon));
        }

        private IEnumerator ActivationRoutine(WeaponController weapon)
        {
            yield return Data.BehaviourBase.ActivationRoutine(weapon);
        }

        public void UpdateTarget(Target target)
        {
            if (target == null)
            {
                transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.right = target.transform.position - transform.position;
            }
        }
    }
}