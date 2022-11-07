using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Tromagon.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Mtl.SagaMap
{
    [PublicAPI]
    public class SagaMapController : MonoBehaviour
    {
        public event Action<int> OnNodeSelected;
    
        [SerializeField] private SagaMapNodeController nodePrefab;
        [SerializeField] private int previousStageCount = 10;
        [SerializeField] private int nextStageCount = 10;
        [SerializeField] private RectTransform content;
        [SerializeField] private ScrollRect scrollRect;
    
        private readonly List<SagaMapNodeController> _stages = new List<SagaMapNodeController>();
    
        public void Init(int topStageIndex)
        {
            var minStage = Mathf.Max(topStageIndex - previousStageCount, 0);
            var maxStage = topStageIndex + nextStageCount;
            for (var i = maxStage; i >= minStage; i--)
            {
                var node = Instantiate(nodePrefab, content);
                var nodeInfo = new NodeInfo
                {
                    Index = i,
                    NodeState = i == topStageIndex ? NodeState.Current
                        : i < topStageIndex ? NodeState.Completed : NodeState.Locked,
                };
    
                node.OnClick += clickedNode =>
                {
                    OnNodeSelected?.Invoke(clickedNode.NodeInfo.Index);
                };
    
                node.Init(nodeInfo);
                _stages.Add(node);
            }
    
            FocusOnStageIndex(topStageIndex);
        }

        public void Clear()
        {
            content.RemoveAllChildren();
            _stages.Clear();
            Canvas.ForceUpdateCanvases();
        }
    
        public void FocusOnStageIndex(int currentStageIndex)
        {
            var stageItem = _stages.Find(x => x.NodeInfo.Index == currentStageIndex);
            SnapTo(stageItem.NodeRect);
        }
    
        private void SnapTo(RectTransform target)
        {
            Canvas.ForceUpdateCanvases();
    
            content.anchoredPosition =
                (Vector2)scrollRect.transform.InverseTransformPoint(content.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
        }
    }
}
