using System;
using UnityEngine;

namespace Mtl.Injection
{
    public class AutoBinder : MonoBehaviour
    {
        [SerializeField] private BindInfo[] infos;
        
        private void Awake()
        {
            foreach (var bindInfo in infos)
            {
                InjectionContainer.Instance.Injector.Bind(bindInfo.Behaviour, bindInfo.Behaviour.GetType(), bindInfo.Id);
            }
        }
        
        private void OnDestroy()
        {
            foreach (var bindInfo in infos)
            {
                InjectionContainer.Instance.Injector.Unbind(bindInfo.Behaviour, bindInfo.Behaviour.GetType());
            }
        }
        
        [Serializable]
        internal class BindInfo
        {
            [field: SerializeField] public Component Behaviour { get; private set; }
            [field: SerializeField] public string Id { get; private set; }
        }
    }
}