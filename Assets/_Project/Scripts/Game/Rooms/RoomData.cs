using UnityEngine;

namespace Project.Game.Rooms
{
    [CreateAssetMenu(fileName = "Data/Rooms/RoomData", menuName = "RoomData", order = 0)]
    public class RoomData : ScriptableObject
    {
        [field: SerializeField] public Color GroundColor { get; private set; } = Color.white;
        [field: SerializeField] public Color WallColor { get; private set; } = Color.white;
    }
}