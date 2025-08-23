using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mtl.Toolbox;
using UnityEngine;
using UnityEngine.UI;

namespace Mtl.SagaMap
{
    [PublicAPI]
    public class SagaMapController : MonoBehaviour
    {
        public event Action<int> OnNodeSelected;

        [Header("Setup")]
        [SerializeField] private SagaMapNodeController nodePrefab;
        [SerializeField] private RectTransform content;
        [SerializeField] private ScrollRect scrollRect;

        [Header("Range")]
        [SerializeField] private int previousStageCount = 10;
        [SerializeField] private int nextStageCount = 10;

        [Header("Path Styling")]
        [SerializeField] private float pathThickness = 18f;
        [SerializeField] private Color pathColor = new Color(0.10f, 0.45f, 0.70f, 1f);
        [SerializeField] private Sprite pathSprite; // optional; leave null for a solid bar
        [SerializeField] private float pathInsetFromNode = 26f; // trims under the circle

        private readonly List<SagaMapNodeController> _stages = new();
        private readonly List<RectTransform> _pathPool = new();
        private RectTransform _pathsLayer;

        /// <summary>
        /// Build the map and focus the current stage (horizontal scrolling).
        /// </summary>
        public void Init(int topStageIndex)
        {
            if (!content || !scrollRect || !nodePrefab)
            {
                Debug.LogError("[SagaMapController] Missing references (content/scrollRect/nodePrefab).");
                return;
            }

            Clear();
            ConfigureForHorizontal();
            EnsurePathsLayer();

            var minStage = Mathf.Max(topStageIndex - previousStageCount, 0);
            var maxStage = topStageIndex + nextStageCount;

            // Left -> right
            for (int i = minStage; i <= maxStage; i++)
            {
                var node = Instantiate(nodePrefab, content);
                var nodeInfo = new NodeInfo
                {
                    Index = i,
                    NodeState = i == topStageIndex ? NodeState.Current
                                : (i < topStageIndex ? NodeState.Completed : NodeState.Locked),
                };

                node.OnClick += clicked => OnNodeSelected?.Invoke(clicked.NodeInfo.Index);
                node.Init(nodeInfo);
                _stages.Add(node);
            }

            // Let layout compute positions/sizes, then size content and draw paths.
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            EnsureContentDimensions();
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);

            BuildPaths();                 // <<< draw background path segments
            FocusOnStageIndex(topStageIndex);
        }

        public void Clear()
        {
            // destroy nodes
            content.RemoveAllChildren();
            _stages.Clear();

            // destroy pooled paths
            if (_pathPool.Count > 0)
            {
                foreach (var rt in _pathPool)
                    if (rt) Destroy(rt.gameObject);
                _pathPool.Clear();
            }

            if (_pathsLayer) _pathsLayer.DetachChildren();
            Canvas.ForceUpdateCanvases();
        }

        public void FocusOnStageIndex(int currentStageIndex)
        {
            var stageItem = _stages.Find(x => x.NodeInfo.Index == currentStageIndex);
            if (stageItem != null)
                SnapTo(stageItem.NodeRect);
        }

        /// <summary>
        /// Centers target horizontally in the viewport (locks vertical).
        /// </summary>
        private void SnapTo(RectTransform target)
        {
            if (!target) return;

            Canvas.ForceUpdateCanvases();

            var viewport = scrollRect.viewport
                           ? scrollRect.viewport
                           : (RectTransform)scrollRect.transform;

            float contentWidth = content.rect.width;
            float viewportWidth = viewport.rect.width;

            if (contentWidth <= viewportWidth) return;

            Vector3 worldCenter = target.TransformPoint(target.rect.center);
            float targetXInContent = content.InverseTransformPoint(worldCenter).x;

            float offset = targetXInContent - (viewportWidth * 0.5f);
            float normalized = Mathf.Clamp01(offset / (contentWidth - viewportWidth));

            scrollRect.horizontalNormalizedPosition = normalized;

            var ap = content.anchoredPosition;
            content.anchoredPosition = new Vector2(ap.x, 0f);
        }

        /// <summary>
        /// Ensures ScrollRect & Content are configured for horizontal scrolling.
        /// </summary>
        private void ConfigureForHorizontal()
        {
            scrollRect.horizontal = true;
            scrollRect.vertical   = false;

            content.anchorMin = new Vector2(0f, 0.5f);
            content.anchorMax = new Vector2(0f, 0.5f);
            content.pivot     = new Vector2(0f, 0.5f);

            var h = content.GetComponent<HorizontalLayoutGroup>();
            if (!h) h = content.gameObject.AddComponent<HorizontalLayoutGroup>();
            h.childAlignment = TextAnchor.MiddleLeft;
            h.childControlWidth = false;
            h.childControlHeight = false;
            h.childForceExpandWidth = false;
            h.childForceExpandHeight = false;
            if (h.spacing <= 0f) h.spacing = 32f;

            var fitter = content.GetComponent<ContentSizeFitter>();
            if (!fitter) fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit   = ContentSizeFitter.FitMode.Unconstrained;

            content.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// Create a dedicated background layer for path segments that the layout system ignores.
        /// </summary>
        private void EnsurePathsLayer()
        {
            if (_pathsLayer) return;

            var go = new GameObject("PathsLayer", typeof(RectTransform));
            _pathsLayer = go.GetComponent<RectTransform>();
            _pathsLayer.SetParent(content, false);

            _pathsLayer.anchorMin = Vector2.zero;
            _pathsLayer.anchorMax = Vector2.one;
            _pathsLayer.pivot     = new Vector2(0.5f, 0.5f);
            _pathsLayer.offsetMin = Vector2.zero;
            _pathsLayer.offsetMax = Vector2.zero;

            var le = go.AddComponent<LayoutElement>();
            le.ignoreLayout = true;

            _pathsLayer.SetAsFirstSibling(); // render behind nodes
        }

        /// <summary>
        /// If layout data isn't providing width yet, give Content a sane width/height.
        /// </summary>
        private void EnsureContentDimensions()
        {
            var viewport = scrollRect.viewport
                           ? scrollRect.viewport
                           : (RectTransform)scrollRect.transform;

            float viewportW = viewport.rect.width  > 0 ? viewport.rect.width  : 800f;
            float viewportH = viewport.rect.height > 0 ? viewport.rect.height : 800f;

            float height = viewportH;

            float width = content.rect.width;
            if (width <= viewportW)
            {
                float estimate = 0f;
                float spacing = 0f;
                var h = content.GetComponent<HorizontalLayoutGroup>();
                if (h) spacing = h.spacing;

                for (int i = 0; i < _stages.Count; i++)
                {
                    var r = _stages[i].NodeRect;
                    float w = r ? r.rect.width : 200f;
                    estimate += w;
                    if (i < _stages.Count - 1) estimate += spacing;
                }
                estimate += 100f;
                width = Mathf.Max(viewportW + 1f, estimate);
            }

            content.sizeDelta = new Vector2(width, height);
        }

        /// <summary>
        /// Draw horizontal path segments behind nodes.
        /// </summary>
        private void BuildPaths()
        {
            if (_stages.Count < 2) return;

            // Clean previous
            foreach (var rt in _pathPool)
                if (rt) Destroy(rt.gameObject);
            _pathPool.Clear();

            // For each gap between nodes, place a thin Image on the paths layer.
            for (int i = 0; i < _stages.Count - 1; i++)
            {
                var a = _stages[i].NodeRect;
                var b = _stages[i + 1].NodeRect;
                if (!a || !b) continue;

                // Centers in content space
                Vector2 aCenter = (Vector2)content.InverseTransformPoint(a.TransformPoint(a.rect.center));
                Vector2 bCenter = (Vector2)content.InverseTransformPoint(b.TransformPoint(b.rect.center));

                // Trim so the bar doesn't run under the circles
                float trim = Mathf.Max(0f, pathInsetFromNode);
                float half = (bCenter.x - aCenter.x) * 0.5f;
                Vector2 left = new Vector2(aCenter.x + trim, aCenter.y);
                Vector2 right = new Vector2(bCenter.x - trim, bCenter.y);

                float width = Mathf.Max(0f, right.x - left.x);
                if (width <= 0.01f) continue;

                var seg = CreatePathImage();
                seg.SetParent(_pathsLayer, false);

                // Position in the middle of the gap
                seg.anchoredPosition = new Vector2((left.x + right.x) * 0.5f, (left.y + right.y) * 0.5f);
                seg.sizeDelta = new Vector2(width, pathThickness);
                seg.localRotation = Quaternion.identity;

                _pathPool.Add(seg);
            }
        }

        private RectTransform CreatePathImage()
        {
            var go = new GameObject("PathSegment", typeof(RectTransform), typeof(Image));
            var rt = go.GetComponent<RectTransform>();
            var img = go.GetComponent<Image>();
            img.raycastTarget = false;
            img.sprite = pathSprite;
            img.type = pathSprite ? Image.Type.Sliced : Image.Type.Simple;
            img.color = pathColor;
            return rt;
        }
    }
}
