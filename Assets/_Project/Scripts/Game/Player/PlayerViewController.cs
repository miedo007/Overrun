using System;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerViewController : MonoBehaviour
    {
        private static readonly int MoveSpeed = Animator.StringToHash("move_speed");
        [field: SerializeField] public PlayerCharacter Character { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public SpriteRenderer BodySprite { get; private set; }

        private void LateUpdate()
        {
            Animator.SetFloat(MoveSpeed, Character.MovementSpeed);
            BodySprite.flipX = Character.HorizontalDirection == -1;
        }
    }
}