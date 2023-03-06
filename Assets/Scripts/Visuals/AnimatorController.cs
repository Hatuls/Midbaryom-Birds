using Midbaryom.Core;
using System;
using UnityEngine;
namespace Midbaryom.Visual
{
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