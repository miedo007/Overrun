using UnityEngine;

namespace Project.Game.Tutorials
{
    public abstract class IdleHintBase : MonoBehaviour
    {
        [Header("Who to watch (movement)")]
        [SerializeField] protected Transform target;

        [Header("Behaviour")]
        [SerializeField] protected float moveThreshold = 0.02f;
        [SerializeField] protected float idleDelay = 2f;
        [SerializeField] protected float fadeDuration = 0.15f;

        [Header("Wave-based Display")]
        [SerializeField] protected float cooldownAfterFirstWave = 10f;

        protected CanvasGroup cg;
        protected Vector3 lastPos;
        protected float idleTimer;
        protected float cooldownTimer;
        protected bool isVisible;
        protected bool tracking;
        protected int currentWaveIndex;
        protected bool cooldownActive;

        public abstract void Begin(int waveIndex = 0);
        public abstract void End();

        protected virtual void Awake()
        {
            cg = GetComponent<CanvasGroup>();
            if (!cg) cg = gameObject.AddComponent<CanvasGroup>();
            HideImmediate();
        }

        protected virtual void Update()
        {
            if (!tracking || !target) return;

            if (cooldownActive)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0f)
                {
                    cooldownActive = false;
                }
                return;
            }

            Vector3 pos = target.position;
            float speed = (pos - lastPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            lastPos = pos;

            if (speed > moveThreshold)
            {
                idleTimer = 0f;
                if (isVisible) Hide();
            }
            else
            {
                idleTimer += Time.deltaTime;
                if (!isVisible && idleTimer >= idleDelay)
                    Show();
            }
        }

        protected virtual void Show()
        {
            isVisible = true;
            StopAllCoroutines();
            StartCoroutine(FadeTo(1f));
        }

        protected virtual void Hide()
        {
            isVisible = false;
            StopAllCoroutines();
            StartCoroutine(FadeTo(0f));
        }

        protected virtual void ShowImmediate()
        {
            isVisible = true;
            StopAllCoroutines();
            cg.alpha = 1f;
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }

        protected virtual void HideImmediate()
        {
            isVisible = false;
            StopAllCoroutines();
            cg.alpha = 0f;
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }

        protected virtual System.Collections.IEnumerator FadeTo(float targetAlpha)
        {
            cg.blocksRaycasts = targetAlpha >= 1f;
            cg.interactable = targetAlpha >= 1f;

            if (fadeDuration <= 0f) { cg.alpha = targetAlpha; yield break; }

            float start = cg.alpha, t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                cg.alpha = Mathf.Lerp(start, targetAlpha, t / fadeDuration);
                yield return null;
            }
            cg.alpha = targetAlpha;
        }
    }
}