using System.Collections.Generic;
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

        [SerializeField]
        private TagSO[] _tags;

        public IEnumerable<TagSO> Tags
        {
            get
            {
                for (int i = 0; i < _tags.Length; i++)
                    yield return _tags[i];
            }
        }
        public Entity Entity => _entity;
        public HoldingOffset HoldingOffset => _holdingOffset;

    }
    [System.Serializable]
    public class HoldingOffset
    {
        public Vector3 PositionOffset;
        [SerializeField]
        private Vector3 _rotationOffset;
        public Quaternion RotaionOffset { get => Quaternion.Euler(_rotationOffset); set => _rotationOffset = value.eulerAngles; }
    }
}