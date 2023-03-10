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
        private const float SMALL_OFFSET = 0.05f;
        public static Spawner Instance;

        private static List<IEntity> _activeEntities = new List<IEntity>();
        [SerializeField]
        private PoolManager _poolManager;

        [SerializeField]
        private SpawnConfigSO _spawnConfig;
        [SerializeField]
        private float _spawningHeightOffset;
        [SerializeField]
        private TagSO _playerTag;

        [SerializeField]
        private bool _toStopReposition;

        private HeightConfigSO _heightConfigSO;
        private IEntity _player;
        UnityEngine.Camera _camera;

        public IReadOnlyList<IEntity> AllEntities => _activeEntities;

        public IEntity Player 
        { 
            get 
            { 
                 if(_player == null)
                    _player = AllEntities.First(x => x.ContainTag(_playerTag));
                return _player; 
            } 
        }

        public static void RegisterEntity(IEntity entity)
            => _activeEntities.Add(entity);
        public static void RemoveEntity(IEntity entity)
            => _activeEntities.Remove(entity);

        public void Awake()
        {
        Instance = this;
            
        }
        private void Start()
        {
            _heightConfigSO = GameManager.Instance.HeightConfigSO;
            _player = Player;
            _camera = UnityEngine.Camera.main;
        }


        private void FixedUpdate()
        {
            // check if there is enough mobs on the map
            // if not then spawn new mob
            if (AllEntities.Count - 1 < _spawnConfig.MaxMobsCount)
                SpawnEntity();

            // check all mobs locations and distance from the player
            // any far mob will need to be repositioned;
            if(!_toStopReposition)
            RePositionMob();
        }

        private void RePositionMob()
        {
            List<IEntity> tooFarEntities = new List<IEntity>();
            for (int i = 0; i < AllEntities.Count; i++)
            {
                Vector3 playerPosition = _player.CurrentPosition;
                Vector3 currentPosition = AllEntities[i].CurrentPosition;
                playerPosition.y = 0;
                currentPosition.y = 0;
                if (Vector3.Distance(currentPosition, playerPosition) > _spawnConfig.ReturnRadius)
                    tooFarEntities.Add(AllEntities[i]);
            }

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

            float yPos = _heightConfigSO.GetHeight(mobTag.StartingHeight).Height + _spawningHeightOffset;
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

                if (Physics.Raycast(_camera.transform.position, directionBetween, out depthCheck, distance + SMALL_OFFSET))
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