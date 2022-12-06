using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Game.Shop
{
    public class EmptySlot : MonoBehaviour, IPointerClickHandler
    {
        public event Action<EmptySlot> Clicked;
        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(this);
        }
    }
}