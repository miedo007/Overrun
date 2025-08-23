using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public class PrefabPool<T> where T : Object
    {
        private readonly Dictionary<T, GenericPool<T>> _pools 
            = new Dictionary<T, GenericPool<T>>();

        private readonly Dictionary<T, GenericPool<T>> _activePrefabs
            = new Dictionary<T, GenericPool<T>>();

        public T Get(T prefab)
        {
            if (!_pools.TryGetValue(prefab, out var pool))
            {
                pool = new GenericPool<T>(() => Object.Instantiate(prefab), 
                    obj =>
                    {
                        var go = GetGameObject(obj);
                        Object.Destroy(go);
                    });
                _pools.Add(prefab, pool);
            }

            var instance = pool.Get();
            var go = GetGameObject(instance); 
            go.SetActive(true);
            _activePrefabs.Add(instance, pool);
            return instance;
        }

        private GameObject GetGameObject(T instance)
        {
            GameObject go = null; 
            switch (instance)
            {
                case GameObject gameObject:
                    go = gameObject;
                    break;
                case MonoBehaviour monoBehaviour:
                    go = monoBehaviour.gameObject;
                    break;
            }

            return go;
        }

        public void Put(T instance)
        {
            var go = GetGameObject(instance); 
            go.SetActive(false);
            var pool = _activePrefabs[instance];
            _activePrefabs.Remove(instance);
            pool.Pool(instance);
        }

        public void Clear()
        {
            foreach (var genericPool in _pools.Values)
            {
                genericPool.ClearPool();
            }
        
            _pools.Clear();
            _activePrefabs.Clear();
        }
    }
}

