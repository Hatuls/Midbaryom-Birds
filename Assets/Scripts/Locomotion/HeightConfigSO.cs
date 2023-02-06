using UnityEngine;

namespace Midbaryom.Core
{
    [CreateAssetMenu(menuName ="ScriptableObjects/Config/New Height Config")]
    public class HeightConfigSO : ScriptableObject
    {
        public HeightTransition PlayerHeight, GroundHeight, AnimalHeight;

        public HeightTransition GetHeight(HeightType heightType)
        {
            switch (heightType)
            {
                case HeightType.Animal:
                    return AnimalHeight;
                case HeightType.Ground:
                    return GroundHeight;
                case HeightType.Player:
                    return PlayerHeight;
                default:
                    throw new System.Exception("Height was not found");
            }
        }
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
        => Evaluate(currentTime, Duration);
        public float Evaluate(float currentTime, float duration)
=> Curve.Evaluate(duration == 0 ? 1 : currentTime / duration);
    }
}