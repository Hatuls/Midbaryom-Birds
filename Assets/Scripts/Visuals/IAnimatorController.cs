using UnityEngine;

namespace Midbaryom.Visual
{
    public interface IAnimatorController
    {
        void PlayAnimation(string animationName);
        void SetFloat(string paramName, float val);
        void SetBool(string paramName, bool val);
        void SetTrigger(string paramName);
    //    Animator Animator { get; }
    }
}