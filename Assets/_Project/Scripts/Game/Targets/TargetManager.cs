using System.Collections.Generic;
using UnityEngine;

namespace Project.Game.Targets
{
    public class TargetManager : MonoBehaviour
    {
        private readonly List<Target> _activeTargets = new List<Target>();
        
        private void Awake()
        {
            Target.StateChanged += OnTargetStateChanged;
        }

        private void OnDestroy()
        {
            Target.StateChanged -= OnTargetStateChanged;
        }

        private void OnTargetStateChanged(Target target)
        {
            if (target.Enabled)
            {
                _activeTargets.Add(target);
            }
            else
            {
                _activeTargets.Remove(target);
            }
        }

        public Target GetClosestTarget(Vector3 position, float range = 5)
        {
            var rangeSqr = range * range;
            
            if (_activeTargets.Count <= 0)
            {
                return null;
            }

            Target closestTarget = null;
            var closestDistSqr = rangeSqr;
            
            for (var i = 0; i < _activeTargets.Count; i++)
            {
                var target = _activeTargets[i];
                var distSqr = (target.transform.position - position).sqrMagnitude;
                if (distSqr <= closestDistSqr)
                {
                    closestTarget = target;
                    closestDistSqr = distSqr;
                }
            }

            return closestTarget;
        }
    }
}