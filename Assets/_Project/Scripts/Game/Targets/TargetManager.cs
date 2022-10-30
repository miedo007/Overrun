using System.Collections.Generic;
using UnityEngine;

namespace Project.Game.Targets
{
    public class TargetManager : MonoBehaviour
    {
        private readonly List<Target> _activeTargets = new List<Target>();
        
        private void Awake()
        {
            Target.Activated += OnTargetActivated;
            Target.Deactivated += OnTargetDeactivated;
        }

        private void OnDestroy()
        {
            Target.Activated -= OnTargetActivated;
            Target.Deactivated -= OnTargetDeactivated;
        }

        private void OnTargetActivated(Target target)
        {
            if (_activeTargets.Contains(target))
            {
                return;
            }
            
            _activeTargets.Add(target);
        }

        private void OnTargetDeactivated(Target target)
        {
            if (!_activeTargets.Contains(target))
            {
                return;
            }
            
            _activeTargets.Remove(target);
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