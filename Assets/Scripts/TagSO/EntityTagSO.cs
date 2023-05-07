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
        [SerializeField]
        private Vector3 _positionOffset;
        public Vector3 PositionOffset
        {
            get
            {
                return _positionOffset;
            }
#if UNITY_EDITOR
            set => _positionOffset = value;
#endif
        }

        [SerializeField]
        private Vector3 _rotationOffset;
        public Quaternion RotaionOffset
        {
            get
            {
                return Quaternion.Euler(_rotationOffset);
            }
#if UNITY_EDITOR
            set => _rotationOffset = value.eulerAngles;
#endif
        }
    }
}