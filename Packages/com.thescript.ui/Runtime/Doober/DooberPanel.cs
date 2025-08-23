using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Mtl.Toolbox;

namespace Mtl.UI
{
    public class DooberPanel : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The tween curve to evaluate for the movement [value: 0-1]")]
        private AnimationCurve _tweenCurve;

        [SerializeField]
        [Tooltip("The default prefab to use when the payload does not specify one")]
        private DooberElement _defaultDooberPrefab;

        private readonly Dictionary<Type, List<IDooberPositionProvider>> _positionProviders = new Dictionary<Type, List<IDooberPositionProvider>>();

        private readonly List<DooberPayload> _activePayloads = new List<DooberPayload>();

        private readonly Dictionary<DooberElement, GenericPool<DooberElement>> _dooberPools;
        
        private Canvas _canvas;
        private RectTransform _rectTransform;

        public DooberPanel()
        {
            _dooberPools = new Dictionary<DooberElement, GenericPool<DooberElement>>();
        }
        
        /// <summary>
        /// Tries to register the provided doober position provider type
        /// </summary>
        /// <param name="provider">The provider to register</param>
        [PublicAPI]
        public void TryRegisterPositionProvider<T>(IDooberPositionProvider<T> provider)
        {
            var type = typeof(T);
            if (!_positionProviders.TryGetValue(type, out var providerList))
            {
                _positionProviders.Add(type, providerList = new List<IDooberPositionProvider>());
            }
            
            providerList.AddUniqueIfNotNull(provider);
            providerList.Sort(SortList);

            int SortList(IDooberPositionProvider l, IDooberPositionProvider r) => -l.DooberProviderPriority.CompareTo(r.DooberProviderPriority);
        }

        /// <summary>
        /// Tries to unregister the provided doober position provider type
        /// </summary>
        /// <returns>True if the provider type was unregistered, false otherwise</returns>
        [PublicAPI]
        public bool UnregisterPositionProvider<T>() => _positionProviders.Remove(typeof(T));

        /// <summary>
        /// Tries to unregister the provided doober position provider reference
        /// </summary>
        /// <param name="provider">The provider to unregister</param>
        /// <returns>True if the provider reference was unregistered, false otherwise</returns>
        [PublicAPI]
        public bool UnregisterPositionProvider<T>(IDooberPositionProvider<T> provider)
        {
            var type = typeof(T);
            return _positionProviders.TryGetValue(type, out var providerList) && providerList.Remove(provider);
        }

        [PublicAPI]
        public void PlayDoober(DooberPayload payload) => PlayDoober(payload, 1, 0f);

        public void PlayDoober(DooberPayload payload, int count, float burstTime)
        {
            var (originPosition, targetPosition) = GetOriginAndTarget(payload);
            if (originPosition == null || targetPosition == null)
            {
                return;
            }

            var routines = ListPool.Get<Coroutine>();
            _activePayloads.Add(payload);
            for (var i = 0; i < count; ++i)
            {
                routines.Add(StartCoroutine(
                    Execute(
                        i / (float) count * burstTime,
                        (i + 1) / (float) count,
                        i == 0)));
            }

            StartCoroutine(WaitForAll(routines));

            IEnumerator WaitForAll(CollectionPools.List<Coroutine> routinesToWait)
            {
                foreach (var routine in routinesToWait)
                {
                    yield return routine;
                }

                if (_activePayloads.Remove(payload))
                {
                    payload.DooberCompleted?.Invoke();
                }

                routinesToWait.Dispose();
            }

            IEnumerator Execute(float delay, float percentage, bool playEndAnim)
            {
                yield return new WaitForSeconds(delay);
                var prefab = payload.Prefab != null ? payload.Prefab : _defaultDooberPrefab;
                var pool = GetOrCreatePool(prefab);
                var element = pool.Get();
                element.SetSprite(payload.Sprite);

                var elementRect = element.RectTransform;
                elementRect.localScale = Vector3.one;
                elementRect.SetSize(payload.Size);

                elementRect.localPosition = originPosition.Value;
                if (prefab.AnimIn != null)
                {
                    yield return elementRect.gameObject.GetComponent<Animator>().PlayClip(prefab.AnimIn);
                }

                var currentTime = 0f;
                var time = _tweenCurve.keys[_tweenCurve.keys.Length - 1].time;
                do
                {
                    elementRect.localPosition = Vector2.Lerp(originPosition.Value, targetPosition.Value, _tweenCurve.Evaluate(currentTime));
                    yield return null;
                } while (EvalTime());

                elementRect.localPosition = targetPosition.Value;
                yield return null;

                payload.DooberReachedTarget?.Invoke(percentage);
                if (playEndAnim && prefab.AnimOut != null)
                {
                    yield return elementRect.gameObject.GetComponent<Animator>().PlayClip(prefab.AnimOut);
                }

                pool.Pool(element);

                bool EvalTime() => (currentTime += Time.deltaTime) < time;
            }
        }

        private (Vector3? origin, Vector3? target) GetOriginAndTarget(DooberPayload payload)
        {
            var originPosition = GetPositionFromProvider(payload.Origin, payload.OriginType, payload.OriginCamera);
            var targetPosition = GetPositionFromProvider(payload.Target, payload.TargetType, payload.TargetCamera);

            if (originPosition == null)
            {
                Debug.LogError($"Non-Fatal: Position is null for origin object '{payload.Origin}' of type '{payload.OriginType}'");
                return (null, null);
            }

            if (targetPosition == null)
            {
                Debug.LogError($"Non-Fatal: Position is null for target object '{payload.Target}' of type '{payload.TargetType}'");
                return (null, null);
            }

            // Reproject into local rect
            var cam = _canvas.rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.rootCanvas.worldCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, originPosition.Value, cam, out var localPoint);
            originPosition = localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, targetPosition.Value, cam, out localPoint);
            targetPosition = localPoint;

            return (originPosition, targetPosition);
        }

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _rectTransform = (RectTransform) transform;

            // Register the default providers
            TryRegisterPositionProvider(new RectTransformProvider());
            TryRegisterPositionProvider(new Vector2Provider());

            if (_defaultDooberPrefab == null)
            {
                _defaultDooberPrefab = DooberElement.Create(transform);
            }

        }

        private void OnDisable()
        {
            // Since coroutines are killed, the objects
            // won't be pooled, so do it manually
            foreach (var (_, pool) in _dooberPools)
            {
                pool.ClearPool();
            }
            
            _dooberPools.Clear();

            // Call cancellation on the payloads that were still active
            foreach (var activePayload in _activePayloads)
            {
                activePayload.DooberCancelled?.Invoke();
            }

            _activePayloads.Clear();
        }

        private GenericPool<DooberElement> GetOrCreatePool(DooberElement prefab)
        {
            if (_dooberPools.TryGetValue(prefab, out var pool))
            {
                return pool;
            }

            pool = new GenericPool<DooberElement>(Create, Dispose, Pool, Unpool);
            _dooberPools.Add(prefab, pool);
            return pool;
            
            DooberElement Create()
            {
                var newInstance = Instantiate(prefab, transform, false);
                newInstance.transform.SetAsFirstSibling();
                newInstance.gameObject.SetActive(true);
                return newInstance;
            }

            void Dispose(DooberElement toDispose) => Destroy(toDispose.gameObject);

            void Pool(DooberElement toPool)
            {
                toPool.SetSprite(null);
                toPool.gameObject.SetActive(false);
            }

            void Unpool(DooberElement toUnpool)
            {
                toUnpool.transform.SetAsFirstSibling();
                toUnpool.gameObject.SetActive(true);
            }
        }

        private Vector2? GetPositionFromProvider(object o, Type type, Camera cam)
        {
            if (!_positionProviders.TryGetValue(type, out var providerList))
            {
                return null;
            }

            foreach (var provider in providerList)
            {
                var position = provider.GetScreenPosition(o, cam);
                if (position.HasValue)
                {
                    return position;
                }
            }

            return null;
        }
    }
}