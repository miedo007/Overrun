using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable PossibleNullReferenceException

namespace Mtl.Injection
{
    [PublicAPI]
    public class Injector
    {
        private readonly Registry _registry = new Registry();
        private readonly IdRegistry _idRegistry = new IdRegistry();
        private readonly InjectTypeCache _injectTypeCache = new InjectTypeCache();
        private readonly List<PendingInjection> _pendingInjections = new List<PendingInjection>();
        private readonly Queue<IInjectionReady> _resolvedQueue = new Queue<IInjectionReady>();

        private string _warningPendingInjectionMessage = "Class {0} is missing injection of type {1}";

        public T Instantiate<T>()
        {
            var obj = Activator.CreateInstance<T>();
            Inject(obj);
            return obj;
        }

        public void Bind(object obj, string id = null)
        {
            Bind(obj, obj.GetType(), id);
        }

        public void Bind<T>(T obj, string id = null)
        {
            Bind(obj, typeof(T), id);
        }

        public void Bind(object obj, Type type, string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                _registry.Add(obj, type);
            }
            else
            {
                _idRegistry.Add(obj, type, id);
            }

            for (var i = _pendingInjections.Count - 1; i >= 0; --i)
            {
                var pendingInjection = _pendingInjections.ElementAt(i);
                if (TryResolveField(pendingInjection.Obj, pendingInjection.InjectFieldInfo))
                {
                    _pendingInjections.RemoveAt(i);
                    if (!HasPendingInjection(pendingInjection.Obj) 
                        && pendingInjection.Obj is IInjectionReady injectionReady)
                    {
                        _resolvedQueue.Enqueue(injectionReady);
                    }
                }
            }
            
            while (_resolvedQueue.Count > 0)
            {
                _resolvedQueue.Dequeue().OnReady();
            }
        }

        public void Unbind(object obj, Type type)
        {
            if (_registry.Contains(obj, type))
            {
                _registry.Remove(obj, type);
                NotifyUnbind(obj);
                return;
            }

            if (!_idRegistry.Contains(obj, type))
                throw new Exception($"Object of type {obj.GetType()} was not registered for injection");
            
            _idRegistry.Remove(obj, type);
            NotifyUnbind(obj);
        }

        public T Get<T>(string id = null)
        {
            var type = typeof(T);
            if (string.IsNullOrEmpty(id))
            {
                return (T)_registry.Get(type);
            }
            
            return (T)_idRegistry.Get(type, id);
        }

        private void NotifyUnbind(object obj)
        {
            if (obj is IUnbind unbind)
            {
                unbind.OnUnbind();
            }
        }

        public void Inject(object obj)
        {
            var type = obj.GetType();
            var injectFieldInfos = _injectTypeCache.GetInjectFieldInfos(type);
            foreach (var injectFieldInfo in injectFieldInfos)
            {
                TryResolveField(obj, injectFieldInfo);
            }
            
            if (!HasPendingInjection(obj) && obj is IInjectionReady injectable)
            {
                injectable.OnReady();
            }
        }

        public bool HasResolvedAll()
        {
            var valid = true;
            foreach (var pendingInjection in _pendingInjections)
            {
                Debug.LogError(string.Format(_warningPendingInjectionMessage, pendingInjection.Obj.GetType(), 
                    pendingInjection.InjectFieldInfo.FieldInfo.FieldType
                    ));

                valid = false;
            }

            return valid;
        }

        private bool TryResolveField(object obj, InjectFieldInfo injectFieldInfo)
        {
            var fieldType = injectFieldInfo.FieldInfo.FieldType;
            var instance = string.IsNullOrEmpty(injectFieldInfo.InjectAttribute.Id) ?
                _registry.Get(fieldType) : _idRegistry.Get(fieldType, injectFieldInfo.InjectAttribute.Id);

            if (instance == null)
            {
                if (_pendingInjections.FindIndex(x => x.Obj == obj && x.InjectFieldInfo == injectFieldInfo) == -1)
                {
                    _pendingInjections.Add(new PendingInjection(obj, injectFieldInfo));
                }

                return false;
            }

            injectFieldInfo.FieldInfo.SetValue(obj, instance);
            return true;
        }

        private bool HasPendingInjection(object obj)
        {
            return _pendingInjections.FindAll(x => x.Obj == obj).Count > 0;
        }

        private class Registry
        {
            private readonly Dictionary<Type, List<object>> _map
                = new Dictionary<Type, List<object>>();
            
            public void Add(object obj, Type type)
            {
                if (!_map.TryGetValue(type, out var list))
                {
                    list = new List<object>();
                    _map.Add(type, list);
                }

                if (list.Contains(obj))
                {
                    throw new Exception($"Object {type} already registered in injector");
                }

                list.Add(obj);
            }

            public void Remove(object obj, Type type)
            {
                if (_map.TryGetValue(type, out var list))
                {
                    if (!list.Contains(obj)) return;
                    list.Remove(obj);
                    if (list.Count == 0)
                    {
                        _map.Remove(type);
                    }
                }
            }

            public object Get(Type type)
            {
                if (!_map.TryGetValue(type, out var list))
                {
                    return null;
                }

                if (list.Count > 1)
                {
                    Debug.LogWarning($"More than one object of type {type} found in injector, return first element");
                }

                return list.ElementAt(0);
            }

            public bool Contains(object obj, Type type)
            {
                return _map.TryGetValue(type, out var list) && list.Contains(obj);
            }
        }

        private class IdRegistry
        {
            private readonly Dictionary<Type, Dictionary<string, object>> _map
                = new Dictionary<Type, Dictionary<string, object>>();

            public void Add(object obj, Type type, string id)
            {
                if (!_map.TryGetValue(type, out var dict))
                {
                    dict = new Dictionary<string, object>();
                    _map.Add(type, dict);
                }

                if (dict.ContainsKey(id))
                {
                    throw new Exception($"Object {type} with id {id} already registered in injector");
                }

                dict.Add(id, obj);
            }
            
            public void Remove(object obj, Type type)
            {
                if (_map.TryGetValue(type, out var dict))
                {
                    if (!dict.ContainsValue(obj)) return;
                    var id = dict.First(x => x.Value == obj).Key;
                    dict.Remove(id);
                    
                    if (dict.Count == 0)
                    {
                        _map.Remove(type);
                    }
                }
            }

            public bool Contains(object obj, Type type)
            {
                return _map.TryGetValue(type, out var dict) && dict.ContainsValue(obj);
            }

            public object Get(Type type, string id)
            {
                if (_map.TryGetValue(type, out var dict) && dict.TryGetValue(id, out var obj)) return obj;
                return null;
            }
        }

        private struct PendingInjection
        {
            public readonly object Obj;
            public readonly InjectFieldInfo InjectFieldInfo;

            public PendingInjection(object obj, InjectFieldInfo injectFieldInfo)
            {
                Obj = obj;
                InjectFieldInfo = injectFieldInfo;
            }
        }
    }
}