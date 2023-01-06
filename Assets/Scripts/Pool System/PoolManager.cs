using Midbaryom.Core;
using System.Collections.Generic;
using UnityEngine;
namespace Midbaryom.Pool
{
    [System.Serializable]
    public class PoolStaterPack
    {
        public EntityTagSO EntityTagSO;
        public int Amount;
    }
    public class PoolManager : MonoBehaviour
    {
        private List<IEntity> _allEntities;
        private List<IEntity> _notActiveEntities;

        [SerializeField]
        private List<PoolStaterPack> _poolStaterPacks;

        public IReadOnlyList<IEntity> AllEntities => _allEntities;

        private void Awake()
        {
            _allEntities = new List<IEntity>();
            _notActiveEntities = new List<IEntity>();
            Init();
        }

        private void Init()
        {
            for (int i = 0; i < _poolStaterPacks.Count; i++)
            {
                PoolStaterPack poolStaterPack = _poolStaterPacks[i];
                int amount = poolStaterPack.Amount;

                for (int j = 0; j < amount; j++)
                    InstantiateEntity(poolStaterPack.EntityTagSO).DestroyHandler.Destroy();
            }
        }

        public IEntity Pull(EntityTagSO tagSO)
        {
            IEntity entity = null;
            for (int i = 0; i < _notActiveEntities.Count; i++)
            {
                if (_notActiveEntities[i].ContainTag(tagSO))
                {
                    entity = _notActiveEntities[i];
                    break;
                }
            }

            if (entity == null)
                entity = InstantiateEntity(tagSO);
            else
                _notActiveEntities.Remove(entity);

            return entity;
        }

        private IEntity InstantiateEntity(EntityTagSO tagSO)
        {
            var cache = Instantiate(tagSO.Entity, transform);
            cache.DestroyHandler.OnDestroy += Return;
            _allEntities.Add(cache);
            return cache;
        }

        private void Return(IEntity obj)
        {
            _notActiveEntities.Add(obj);
        }
    }
}
