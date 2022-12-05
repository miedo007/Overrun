using UnityEngine;

namespace Project.Game.UI
{
    public class MatchReference : MonoBehaviour
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private RectTransform reference;

        private void Update()
        {
            var position = rect.position;
            position.y = reference.position.y;
            rect.position = position;
        }
    }
}