using UnityEngine;
namespace Midbaryom.Camera
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Config/Camera/New Camera Rotation")]
    public class CameraRotationSO : ScriptableObject
    {
        public float Angle;
        public float Duration;
        public AnimationCurve Curve;
        public float Evaluate(float currentTime) => Curve.Evaluate(currentTime / Duration);
    }
}