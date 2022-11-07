using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Mtl.SagaMap
{
    public enum NodeState
    {
        Completed,
        Current,
        Locked
    }

    public struct NodeInfo
    {
        public int Index;
        public NodeState NodeState;
    }

    [PublicAPI]
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
            set
            {
                foreach (var selectionObject in selectionObjects)
                {
                    selectionObject.SetActive(value);
                }
            }
        }

        private void Awake()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            OnClick?.Invoke(this);
        }

        public void Init(NodeInfo nodeInfo)
        {
            NodeInfo = nodeInfo;
            gameObject.name = $"Stage {NodeInfo.Index}";
            foreach (var nodeNumberLabel in nodeNumberLabels)
            {
                nodeNumberLabel.text = $"{NodeInfo.Index + 1:##0}";
            }

            //states
            activeRoot.SetActive(nodeInfo.NodeState == NodeState.Current);
            lockedRoot.SetActive(nodeInfo.NodeState == NodeState.Locked);
            if (nodeInfo.Index != 0) return;

            pathUp.anchorMin = new Vector2(0, 0.5f);
            pathUp.anchorMax = new Vector2(1, 1);
        }
    }
}
