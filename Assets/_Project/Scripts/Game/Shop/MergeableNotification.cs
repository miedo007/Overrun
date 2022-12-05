using UnityEngine;
using UnityEngine.UI;

namespace Project.Game.Shop
{
    public class MergeableNotification : MonoBehaviour
    {
        [field: SerializeField] public Image MergeableNotifImage { get; private set; }

        public void Activate(Color color)
        {
            MergeableNotifImage.color = color;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}