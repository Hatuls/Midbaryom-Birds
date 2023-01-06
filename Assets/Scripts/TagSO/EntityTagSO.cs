using UnityEngine;
namespace Midbaryom.Core
{
    [CreateAssetMenu(menuName ="ScriptableObjects/Tags/New Entity", fileName ="New Entitiy SO")]
    public class EntityTagSO : TagSO {
        [SerializeField]
        private Entity _entity;

        public Entity Entity => _entity;
    }
}