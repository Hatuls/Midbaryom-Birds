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
    public class AnimatorController : IAnimatorController
    {

        [SerializeField]
        private Animator _animator;
   //     public Animator Animator => _animator;
   

        internal void Init(IEntity entity)
        {

        }

        public void PlayAnimation(string animationName)
        {
            _animator.Play(animationName);
        }
        public void SetFloat(string paramName, float val) => _animator.SetFloat(paramName, val);
        public void SetBool(string paramName, bool val) => _animator.SetBool(paramName, val);
        public void SetTrigger(string paramName) => _animator.SetTrigger(paramName);
    }


}