using UnityEngine;
namespace Midbaryom.Core
{
    [CreateAssetMenu(menuName ="ScriptableObjects/Tags/New Entity", fileName ="New Entitiy SO")]
    public class EntityTagSO : TagSO 
    {
        public HeightType StartingHeight;
        [SerializeField]
        private Entity _entity;
        [SerializeField]
        private HoldingOffset _holdingOffset;

        public bool CanBeTargeted = true;

        public Entity Entity => _entity;
        public HoldingOffset HoldingOffset => _holdingOffset;

    }
    [System.Serializable]
    public class HoldingOffset
    {
        public Vector3 PositionOffset;
        [SerializeField]
        private Vector3 _rotationOffset;
        public Quaternion RotaionOffset => Quaternion.Euler(_rotationOffset);
    }
}