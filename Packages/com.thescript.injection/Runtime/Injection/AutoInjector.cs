using UnityEngine;

namespace Mtl.Injection
{
    public class AutoInjector : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] injectedBehaviors;

        private void Awake()
        {
            foreach (var behaviour in injectedBehaviors)
            {
                InjectionContainer.Instance.Injector.Inject(behaviour);
            }
        }
    }
}