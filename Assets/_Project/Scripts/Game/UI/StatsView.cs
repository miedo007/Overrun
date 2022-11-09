using Mtl.Injection;
using Project.Heroes;
using UnityEngine;

namespace Project.Game.UI
{
    public class StatsView : MonoBehaviour
    {
        [SerializeField] private StatView statViewPrefab;
        [SerializeField] private RectTransform parent;
        
        [Inject] private HeroInfo _heroInfo;

        public void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            foreach (var stat in _heroInfo.Stats)
            {
                var statView = Instantiate(statViewPrefab, parent);
                statView.Initialize(stat);
                statView.gameObject.SetActive(true);
            }
        }
    }
}