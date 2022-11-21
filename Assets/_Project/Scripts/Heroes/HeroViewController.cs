using Project.Game.Player;
using Project.Game.UI;
using UnityEngine;

namespace Project.Heroes
{
    public class HeroViewController : MonoBehaviour
    {
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public StatUpgradeList StatUpgradeList { get; private set; }

        private bool _isFlipped;

        private static readonly int MoveSpeed = Animator.StringToHash("move_speed");

        public PlayerCharacter Character { get; private set; }
        public Transform HeroRoot { get; private set; }

        public void Initialize(PlayerCharacter character)
        {
            Character = character;
            HeroRoot = transform;
        }

        private void LateUpdate()
        {
            Animator.SetFloat(MoveSpeed, Character.MovementSpeed);

            if (_isFlipped && Character.HorizontalDirection == 1)
            {
                HeroRoot.localScale = new Vector3(1, 1, 1);
                _isFlipped = false;
            }
            else if (!_isFlipped && Character.HorizontalDirection == -1)
            {
                HeroRoot.localScale = new Vector3(-1, 1, 1);
                _isFlipped = true;
            }
        }
    }
}