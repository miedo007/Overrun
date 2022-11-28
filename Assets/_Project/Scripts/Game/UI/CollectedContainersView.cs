using System.Collections.Generic;
using System.Linq;
using Mtl.Injection;
using Mtl.Toolbox;
using Project.Heroes;
using UnityEngine;

namespace Project.Game.UI
{
    public class CollectedContainersView : MonoBehaviour
    {
        [field: SerializeField] public RectTransform Parent { get; private set; }
        [field: SerializeField] public GameObject ContainerViewPrefab { get; private set; }

        [Inject] private readonly HeroRegistry _heroRegistry;

        private List<GameObject> _containerViews = new();

        private void Start()
        {
            _heroRegistry.ActiveHero.CollectedContainersChanged += OnCollectedContainersChanged;
        }
        
        private void OnDestroy()
        {
            _heroRegistry.ActiveHero.CollectedContainersChanged -= OnCollectedContainersChanged;
        }

        private void OnEnable()
        {
            Parent.RemoveAllChildren();
        }

        private void OnCollectedContainersChanged(int delta)
        {
            if (delta > 0)
            {
                for (var i = 0; i < delta; i++)
                {
                    _containerViews.Add(Instantiate(ContainerViewPrefab, Parent));
                }
            }
            else
            {
                for (int i = 0; i < Mathf.Abs(delta); i++)
                {
                    var containerToRemove = _containerViews.Last();
                    _containerViews.Remove(containerToRemove);
                    Destroy(containerToRemove);
                }
            }
        }
    }
}