using UnityEngine;

namespace Project.Application
{
    public class BaseData : ScriptableObject
    {
        [field: SerializeField] public Sprite Sprite { get; protected set; }
        [field: SerializeField] public string DisplayName { get; protected set; }
        [field: SerializeField, TextArea] public string Description { get; protected set; }
    }
}