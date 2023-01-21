using Midbaryom.Core.Config;
using Midbaryom.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Midbaryom.Core
{

    public class Spawner : MonoBehaviour
    {
        private static List<IEntity> _activeEntities = new List<IEntity>();
        [SerializeField]
        private PoolManager _poolManager;

        [SerializeField]
        private SpawnConfigSO _spawnConfig;

        [SerializeField]
        private EntityTagSO _playerTag;
        private HeightConfigSO _heightConfigSO;
        private IEntity _player;
        UnityEngine.Camera _camera;
        private IReadOnlyList<IEntity> AllEntities => _activeEntities;

        public static void RegisterEntity(IEntity entity)
            => _activeEntities.Add(entity);
        public static void RemoveEntity(IEntity entity)
            => _activeEntities.Remove(entity);

        private void Start()
        {
            _heightConfigSO = GameManager.Instance.HeightConfigSO;
            _player = AllEntities.First(x => x.ContainTag(_playerTag));
            _camera = UnityEngine.Camera.main;
        }


        private void Update()
        {
            // check if there is enough mobs on the map
            // if not then spawn new mob
            if (AllEntities.Count - 1 < _spawnConfig.MaxMobsCount)
                SpawnEntity();

            // check all mobs locations and distance from the player
            // any far mob will need to be repositioned;

            RePositionMob();
        }

        private void RePositionMob()
        {
            List<IEntity> tooFarEntities = new List<IEntity>();
            for (int i = 0; i < AllEntities.Count; i++)
                if (Vector3.Distance(AllEntities[i].CurrentPosition, _player.CurrentPosition) > _spawnConfig.ReturnRadius)
                    tooFarEntities.Add(AllEntities[i]);

            if(tooFarEntities.Count>0)
            {
                for (int i = 0; i < tooFarEntities.Count; i++)
                {
                    tooFarEntities[i].DestroyHandler.Destroy();
                    tooFarEntities[i].Transform.gameObject.SetActive(false);
                }
                tooFarEntities.Clear();
            }
        }

        private void SpawnEntity()
        {
            EntityTagSO mobTag = _spawnConfig.ConductMob(AllEntities);
            IEntity mob = _poolManager.Pull(mobTag);

            float yPos = _heightConfigSO.GetHeight(mobTag.StartingHeight).Height;
            Transform t = mob.Transform;
            t.position = GenerateSpawnLocation(yPos);
            t.gameObject.SetActive(true);
            t.SetParent(null);
        }

        private Vector3 GenerateSpawnLocation(float yPos)
        {
            Vector3 playerPosition = _player.Transform.position;
            Vector3 destination = playerPosition;
            destination.y = yPos;

            do
            {
                destination.x = GetRandomPoint(playerPosition.x);
                destination.z = GetRandomPoint(playerPosition.z);

            } while (PointInCameraView(destination));

            return destination;



            bool PointInCameraView(Vector3 point)
            {
        
                Vector3 viewport = _camera.WorldToViewportPoint(point);
                bool inCameraFrustum = Is01(viewport.x) && Is01(viewport.y);
                bool inFrontOfCamera = viewport.z > 0;

                RaycastHit depthCheck;
                bool objectBlockingPoint = false;

                Vector3 directionBetween = point - _camera.transform.position;
                directionBetween = directionBetween.normalized;

                float distance = Vector3.Distance(_camera.transform.position, point);

                if (Physics.Raycast(_camera.transform.position, directionBetween, out depthCheck, distance + 0.05f))
                {
                    if (depthCheck.point != point)
                    {
                        objectBlockingPoint = true;
                    }
                }

                return inCameraFrustum && inFrontOfCamera && !objectBlockingPoint;
            }

            bool Is01(float a)
            =>a > 0 && a < 1;
            
            float GetRandomPoint(float playerAxisPoint) =>
            UnityEngine.Random.Range(playerAxisPoint - _spawnConfig.SpawnRadius, playerAxisPoint + _spawnConfig.SpawnRadius);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _spawnConfig.ReturnRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _spawnConfig.SpawnRadius);

        }
    }

}