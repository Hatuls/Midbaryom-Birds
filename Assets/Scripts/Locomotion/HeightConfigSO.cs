using UnityEngine;

namespace Midbaryom.Core
{
    [CreateAssetMenu(menuName ="ScriptableObjects/Config/New Height Config")]
    public class HeightConfigSO : ScriptableObject
    {
        public HeightTransition PlayerHeight, GroundHeight, AnimalHeight;
    }

    [System.Serializable]
    public class HeightTransition
    {
        public float Height;
        [SerializeField]
        private AnimationCurve Curve;
        [SerializeField]
        private float Duration;

        public float Evaluate(float currentTime)
        => Curve.Evaluate(currentTime / Duration);
    }
}