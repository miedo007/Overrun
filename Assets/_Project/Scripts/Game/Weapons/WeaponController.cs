using System;
using System.Collections;
using Project.Game.Targets;
using Unity.Mathematics;
using UnityEngine;

namespace Project.Game.Weapons
{
    public class WeaponController : MonoBehaviour
    {
        public event Action Activated;
        
        [field: SerializeField] public Transform Barrel { get; set; }
        
        private float _lastActivationTime;
        private Transform _transform;
        
        public WeaponData Data { get; private set; }

        private void Awake()
        {
            _transform = transform;
        }

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
                _transform.rotation = Quaternion.identity;
                _transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                // vector from this object towards the target location
                var vectorToTarget = (target.transform.position - _transform.position).normalized;
                // rotate that vector by 90 degrees around the Z axis
                var rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;
                
                var targetRotation = Quaternion.LookRotation(Vector3.forward, rotatedVectorToTarget);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1080 * Time.deltaTime);
                _transform.localScale = target.transform.position.x < _transform.position.x 
                    ? new Vector3(1, -1, 1)
                    : new Vector3(1,1,1);
            }
        }
    }
}