using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mtl.ContextManagement;

namespace Mtl.Injection
{
    [PublicAPI]
    public class InjectContext : Context
    {
        private Injector _injector;
        private List<BindInfo> _activeBinds;

        private struct BindInfo
        {
            public object Obj;
            public Type Type;
        }

        protected sealed override void OnSetup()
        {
            _activeBinds = new List<BindInfo>();
            _injector = InjectionContainer.Instance.Injector;
            OnInjectStart();
        }

        protected override void OnPostSetup()
        {
            _injector.HasResolvedAll();
        }

        protected virtual void OnInjectStart()
        {
        }

        protected override void OnContextDestroy()
        {
            foreach (var bindInfo in _activeBinds)
            {
                _injector.Unbind(bindInfo.Obj, bindInfo.Type);
            }
        }

        protected T Get<T>(string id = null)
        {
            return _injector.Get<T>(id);
        }

        public T Instantiate<T>()
        {
            return _injector.Instantiate<T>();
        }

        public void Inject(object obj)
        {
            _injector.Inject(obj);
        }

        public T InstantiateAndBind<T>(string id = null)
        {
            var obj = _injector.Instantiate<T>();
            Bind(obj, id);
            return obj;
        }

        public void InjectAndBind(object obj, string id = null)
        {
            _injector.Inject(obj);
            Bind(obj, id);
        }

        public void InjectAndBind<T>(T obj, string id = null)
        {
            _injector.Inject(obj);
            Bind(obj, id);
        }

        public void Bind(object obj, string id = null)
        {
            Bind(obj, obj.GetType(), id);
        }

        public void Bind<T>(T obj, string id = null)
        {
            Bind(obj, typeof(T), id);
        }

        private void Bind(object obj, Type type, string id = null)
        {
            _injector.Bind(obj, type, id);
            _activeBinds.Add(new BindInfo
            {
                Obj = obj,
                Type = type
            });
        }
    }
}