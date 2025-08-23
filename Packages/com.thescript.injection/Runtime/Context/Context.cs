using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.ContextManagement
{
    [PublicAPI]
    public class Context : MonoBehaviour
    {
        private void Awake()
        {
            ProjectInitializer.Instance.Initialize(OnSetup);
        }

        private void Start()
        {
            OnPostSetup();
        }

        private void OnDestroy()
        {
            ProjectInitializer.Instance.NotifyContextDestroyed(gameObject);
            OnContextDestroy();
        }

        protected virtual void OnSetup()
        {
        }
        
        protected virtual void OnPostSetup()
        {
        }

        protected virtual void OnContextDestroy()
        {
            
        }
    }
}

