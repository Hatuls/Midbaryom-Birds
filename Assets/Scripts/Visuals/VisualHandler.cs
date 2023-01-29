using Midbaryom.Core;
using System;
using UnityEngine;
namespace Midbaryom.Visual
{
    public interface IVisualHandler
    {
        IAnimatorController AnimatorController { get; }
        Transform VisualTransform { get; }
        void Init(IEntity entity);
    }

    public class VisualHandler : MonoBehaviour, IVisualHandler
    {
        [SerializeField]
        private Transform _visualTransform;
        [SerializeField]
        private AnimatorController _animatorController;
        public IAnimatorController AnimatorController => _animatorController;

        public Transform VisualTransform { get => _visualTransform; }

        public void Init(IEntity entity)
        {
            _animatorController.Init(entity);
        }

        private void LateUpdate()
        {
            _visualTransform.transform.localPosition = Vector3.zero;
        }
    }
    [Serializable]
    public class AnimatorController : IAnimatorController, IBehaviour
    {

        [SerializeField]
        private Animator _animator;
        public Animator Animator => _animator;
        private IRotator _rotator;
        private ILocomotion _locomotion;
        private IStat _movmenetSpeed;

        internal void Init(IEntity entity)
        {
            _movmenetSpeed = entity.StatHandler[StatType.MovementSpeed];
            _locomotion = entity.MovementHandler;
            _rotator = entity.Rotator;
            _rotator.OnFaceDirection += AssignRotation;
            _locomotion.OnMove += AssignMovementSpeed;
            entity.DestroyHandler.AddBehaviour(this);
        }

        private void AssignRotation(float angle)
        {
            // Animator.SetFloat(RotationHash, -angle);
        }

        private void AssignMovementSpeed(float speed)
        {
            //float startSpeed = _movmenetSpeed.StartValue;
            //speed /= startSpeed * 2;
            //Animator.SetFloat("Forward", speed);

        }

        public void ApplyBehaviour()
        {
            _rotator.OnFaceDirection -= AssignRotation;
            _locomotion.OnMove -= AssignMovementSpeed;
        }
    }


}