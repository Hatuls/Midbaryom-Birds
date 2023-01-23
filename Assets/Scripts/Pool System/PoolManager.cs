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
        private static PoolManager _instance;
        private List<IEntity> _activeEntities;
        private List<IEntity> _notActiveEntities;

        [SerializeField]
        private List<PoolStaterPack> _poolStaterPacks;
        public static PoolManager Instance
        {
            get
            {
                return _instance;
            }
        }
        public IReadOnlyList<IEntity> ActiveEntities => _activeEntities;

        private void Awake()
        {
            _instance = this;
            Init();
        }

        private void Init()
        {
            _activeEntities = new List<IEntity>();
            _notActiveEntities = new List<IEntity>();

            if (_poolStaterPacks != null)
            {

                for (int i = 0; i < _poolStaterPacks.Count; i++)
                {
                    PoolStaterPack poolStaterPack = _poolStaterPacks[i];
                    int amount = poolStaterPack.Amount;

                    for (int j = 0; j < amount; j++)
                    {
                        var entity = InstantiateEntity(poolStaterPack.EntityTagSO);
                            entity.DestroyHandler.Destroy();
                    }
                }
                ReturnAllBack();
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

            _activeEntities.Add(entity);
            return entity;
        }

        private IEntity InstantiateEntity(EntityTagSO tagSO)
        {
            var cache = Instantiate(tagSO.Entity, transform);
            cache.DestroyHandler.OnDestroy += Return;
            return cache;
        }

        private void Return(IEntity obj)
        {
            _activeEntities.Remove(obj);
            _notActiveEntities.Add(obj);
            obj.Transform.gameObject.SetActive(false);
        }
        public void ReturnAllBack()
        {
            for (int i = 0; i < ActiveEntities.Count; i++)
            {
                Return(ActiveEntities[i]);
            }
        }
        private void OnDestroy()
        {

            foreach (var entity in DestroyHandlers())
                entity.OnDestroy -= Return;

            IEnumerable<IDestroyHandler> DestroyHandlers()
            {
                if(ActiveEntities != null && ActiveEntities.Count>0)
                for (int i = 0; i < ActiveEntities.Count; i++)
                    yield return ActiveEntities[i].DestroyHandler;

                if(_notActiveEntities!=null && _notActiveEntities.Count>0)
                for (int i = 0; i < _notActiveEntities.Count; i++)
                    yield return _notActiveEntities[i].DestroyHandler;

            }
        }
    }
}
