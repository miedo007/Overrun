using Mtl.Injection;
using Project.Heroes;
using UnityEngine;

namespace Project.Game.UI
{
    public class StatsView : MonoBehaviour
    {
        [SerializeField] private StatView statViewPrefab;
        [SerializeField] private RectTransform parent;
        
        private HeroInfo _heroInfo;

        public void Start()
        {
            _heroInfo = InjectionContainer.Instance.Injector.Get<HeroInfo>();
            Debug.Log("Stats view setup");
            foreach (var stat in _heroInfo.Stats)
            {
                var statView = Instantiate(statViewPrefab, parent);
                statView.Initialize(stat);
            }
        }
    }
}