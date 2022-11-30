using Mtl.Injection;
using Project.Feedback;
using Project.Game.Player;
using Project.Game.UI;
using UnityEngine;

namespace Project.Heroes
{
    public class HeroViewController : MonoBehaviour, IInjectionReady
    {
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public StatUpgradeList StatUpgradeList { get; private set; }
        [field: SerializeField] public FeedbackData DamageTakenFeedback { get; private set; }

        private bool _isFlipped;

        private static readonly int MoveSpeed = Animator.StringToHash("move_speed");

        [Inject] private readonly PlayerHealthController _playerHealthController;
        

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

        public void OnReady()
        {
            _playerHealthController.DamageTaken += OnDamageTaken;
        }

        private void OnDamageTaken()
        {
            DamageTakenFeedback.Play(transform.position, Quaternion.identity);
        }
    }
}