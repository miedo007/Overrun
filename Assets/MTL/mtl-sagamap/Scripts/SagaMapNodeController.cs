using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Mtl.SagaMap
{
    public enum NodeState { Completed, Current, Locked }

    public struct NodeInfo
    {
        public int Index;
        public NodeState NodeState;
    }

    public class SagaMapNodeController : MonoBehaviour
    {
        public event Action<SagaMapNodeController> OnClick;

        [SerializeField] private List<TextMeshProUGUI> nodeNumberLabels;
        [SerializeField] private GameObject activeRoot;
        [SerializeField] private GameObject lockedRoot;
        [SerializeField] private RectTransform pathUp;
        [SerializeField] private List<GameObject> selectionObjects;
        [SerializeField] private Button button;
        [field: SerializeField] public RectTransform NodeRect { get; private set; }

        public NodeInfo NodeInfo { get; private set; }

        public bool IsSelected
        {
            set { foreach (var o in selectionObjects) o.SetActive(value); }
        }

        private void Awake()
        {
            if (button) button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked() => OnClick?.Invoke(this);

        public void Init(NodeInfo nodeInfo)
        {
            NodeInfo = nodeInfo;
            gameObject.name = $"Stage {NodeInfo.Index}";

            foreach (var lbl in nodeNumberLabels)
                lbl.text = $"{NodeInfo.Index + 1:##0}";

            activeRoot.SetActive(nodeInfo.NodeState == NodeState.Current);
            lockedRoot.SetActive(nodeInfo.NodeState == NodeState.Locked);

            if (nodeInfo.Index == 0 && pathUp)
            {
                pathUp.anchorMin = new Vector2(0f, 0.5f);
                pathUp.anchorMax = new Vector2(1f, 1f);
            }
        }

        /// <summary>
        /// Move the path segment to a background container while preserving world position.
        /// Also disables raycasts and ignores layout so it won’t be moved.
        /// </summary>
        public RectTransform DetachPathTo(RectTransform newParent)
        {
            if (!pathUp || !newParent) return null;

            // Don’t block clicks
            var g = pathUp.GetComponent<Graphic>();
            if (g) g.raycastTarget = false;

            // Make sure no layout group will try to position this
            var le = pathUp.GetComponent<LayoutElement>();
            if (!le) le = pathUp.gameObject.AddComponent<LayoutElement>();
            le.ignoreLayout = true;

            pathUp.SetParent(newParent, true); // keep world position
            return pathUp;
        }
    }
}
