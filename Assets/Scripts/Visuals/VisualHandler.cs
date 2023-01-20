using Midbaryom.Core;
using System;
using UnityEngine;
namespace Midbaryom.Visual
{

    public class VisualHandler : MonoBehaviour
    {
        [SerializeField]
        private AnimatorController _animatorController;
        public IAnimatorController AnimatorController => _animatorController;

        public void Init(IEntity entity)
        {
            _animatorController.Init(entity);
        }
    }
    [Serializable]
    public class AnimatorController : IAnimatorController, IBehaviour
    {
        [SerializeField]
        private Animator _animator;
        public Animator Animator => _animator;

        private IStat _movementSpeed;
        private int MovementSpeedHash = Animator.StringToHash("Speed");
        internal void Init(IEntity entity)
        {
            _movementSpeed = entity.StatHandler[StatType.MovementSpeed];
         _movementSpeed.OnValueChanged += AssignMovementSpeed;
            entity.DestroyHandler.AddBehaviour(this);
        }


        private void AssignMovementSpeed(float speed)
        {
            Animator.SetFloat(MovementSpeedHash, speed);
        }

        public void ApplyBehaviour()
        {
            _movementSpeed.OnValueChanged -= AssignMovementSpeed;
        }
    }


}