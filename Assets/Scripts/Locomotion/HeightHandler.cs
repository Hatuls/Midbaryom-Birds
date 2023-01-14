using UnityEngine;

namespace Midbaryom.Core
{
    public class HeightHandler : IHeightHandler
    {
        private readonly HeightConfigSO HeightConfigSO;
        private readonly Locomotion Locomotion;
        private readonly Transform Transform;
        public HeightHandler(Locomotion locomotion, Transform transform)
        {
            Locomotion = locomotion;
            Transform = transform;
            HeightConfigSO = GameManager.Instance.HeightConfigSO;
            Locomotion.OnHeightRequested += CalculateHeight;
        }
        ~HeightHandler()
        {
            Locomotion.OnHeightRequested -= CalculateHeight;
        }
        public float GetHeight()
        => Transform.position.y;


        public void SetHeight(Vector3 height) => SetHeight(height.y);

        public void SetHeight(float height)
        {
            Vector3 currentPos = Transform.position;
            currentPos.y = height;
            Transform.position = currentPos;
        }
        public Vector3 CalculateHeight() => HeightConfigSO.AnimalHeight * Vector3.up;
    }

    public interface IHeightHandler
    {
        void SetHeight(Vector3 height);
        void SetHeight(float height);
        Vector3 CalculateHeight();
        float GetHeight();
    }
}