using UnityEngine;

namespace Mtl.UI
{
    public class WorldTargetPin : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private GameObject guiElement;
        [SerializeField] [Range(0f, 1f)] private float smoothFactor;
        [SerializeField] private Transform overrideTarget;
        [SerializeField] private Vector3 offset;

        private Camera _activeCamera;
        private Transform _currentTarget;

        public void SetCanvas(Canvas value)
        {
            canvas = value;
        }

        public void SetCamera(Camera value)
        {
            _activeCamera = value;
        }

        public void SetTarget(Transform newTarget)
        {
            _currentTarget = newTarget;
        }

        private void LateUpdate()
        {
            var target = overrideTarget ? overrideTarget : _currentTarget;
            if (target)
            {
                PlaceRectAtWorldPosition(target);
            }
        }

        private void PlaceRectAtWorldPosition(Transform target)
        {
            if (canvas == null) return;
            var targetPosition = target.transform.position;

            if (_activeCamera == null)
            {
                _activeCamera = Camera.main;
            }

            if (_activeCamera == null) return;

            var finalPos = _activeCamera.WorldToScreenPoint(targetPosition + offset) / canvas.scaleFactor;
            if (finalPos.z < 0)
            {
                guiElement.SetActive(false);
                return;
            }

            guiElement.SetActive(true);
            var scaleFactor = canvas.scaleFactor;
            var screenWidthFromCenter = (int)(Screen.width / scaleFactor) / 2;
            var screenHeightFromCenter = (int)(Screen.height / scaleFactor) / 2;

            //"convert" this value to bottom left of parent (i.e. pretend that anchor = 0,0)
            var anchorMax = rectTransform.anchorMax;
            var bottomLeftOfParent = new Vector2(-anchorMax.x * 2 * screenWidthFromCenter,
                -anchorMax.y * 2 * screenHeightFromCenter);

            //move the "converted" value to targetPosition
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition,
                bottomLeftOfParent + new Vector2(finalPos.x, finalPos.y), smoothFactor);
        }
    }
}