using UnityEngine;

namespace Mtl.Injection
{
    public class InjectionContainer
    {
        public static InjectionContainer Instance;

        public Injector Injector { get; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReloadInstance()
        {
            Instance = new InjectionContainer();
        }

        private InjectionContainer()
        {
            Injector = new Injector();
        }
    }
}