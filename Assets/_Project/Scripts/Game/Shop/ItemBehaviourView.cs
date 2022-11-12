using Project.Game.Items;
using TMPro;
using UnityEngine;

namespace Project.Game.Shop
{
    public class ItemBehaviourView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI descriptionField;
        
        public void Initialize(BehaviourTriggerPair behaviourPair)
        {
            descriptionField.text = $"{behaviourPair.Trigger.Description}, {behaviourPair.Behaviour.GetDescription()}";
        }
    }
}