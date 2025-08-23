using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace Mtl.Toolbox
{
    [PublicAPI]
    public static class GameObjectExtensions
    {
        public static void SetAllActive(this IEnumerable<GameObject> objs, bool active)
        {
            foreach (var obj in objs)
            {
                obj.SetActive(active);
            }
        }

        #region Threading

        public static async Task<T> GetComponentAsync<T>(this GameObject go) => 
            await UnityThreading.ExecuteOnMainThread(go.GetComponent<T>);

        public static async Task<T> GetComponentInChildrenAsync<T>(this GameObject go) => 
            await UnityThreading.ExecuteOnMainThread(go.GetComponentInChildren<T>);

        public static async Task<T> GetComponentInParentAsync<T>(this GameObject go) => 
            await UnityThreading.ExecuteOnMainThread(go.GetComponentInParent<T>);

        public static async Task<T[]> GetComponentsAsync<T>(this GameObject go) => 
            await UnityThreading.ExecuteOnMainThread(go.GetComponents<T>);

        public static async Task<T[]> GetComponentsInChildrenAsync<T>(this GameObject go) => 
            await UnityThreading.ExecuteOnMainThread(go.GetComponentsInChildren<T>);

        public static async Task<T[]> GetComponentsInParentAsync<T>(this GameObject go) => 
            await UnityThreading.ExecuteOnMainThread(go.GetComponentsInParent<T>);

        #endregion
    }
}