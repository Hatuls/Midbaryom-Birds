using Midbaryom.Core.Config;
using Midbaryom.Pool;
using System.Collections;
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
        public float _tickTime = 0.002f;
        private HeightConfigSO _heightConfigSO;
        private IEntity _playerEntity;
        private IPlayer _player;
        private UnityEngine.Camera _camera;
        private bool _startSpawning;
        [SerializeField]
        private AimAssists _aimAssists;


        [SerializeField]
        private float _maxViewPort = 1f;
        [SerializeField]
        private float _minViewPort = 0f;

        public IReadOnlyList<IEntity> AllEntities => _activeEntities;

        public IEntity PlayerEntity
        {
            get
            {
                if (_playerEntity == null)
                    _playerEntity = AllEntities.First(x => x.ContainTag(_playerTag));
                return _playerEntity;
            }
        }
        public IPlayer Player
        {
            get
            {
                if (_player == null)
                    _player = PlayerEntity.Transform.GetComponent<IPlayer>();
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
            _startSpawning = false;
            GameManager.OnGameStarted += StartGame;
        }
        private void Start()
        {
            _heightConfigSO = GameManager.Instance.HeightConfigSO;
            _playerEntity = PlayerEntity;
            _camera = UnityEngine.Camera.main;
            StartCoroutine(SpawnCoroutine());
        }

        private void OnDestroy()
        {
            GameManager.OnGameStarted -= StartGame;
            StopAllCoroutines();
        }



        private IEnumerator SpawnCoroutine()
        {
            var yieldInstructions = new WaitForSeconds(_tickTime);
            while (true)
            {

                if (!_startSpawning)
                {
                    yield return null;
                    continue;
                }
                else
                    yield return yieldInstructions;
                // check if there is enough mobs on the map
                // if not then spawn new mob
                if (AllEntities.Count - 1 < _spawnConfig.MaxMobsCount)
                    SpawnEntity();

                // check all mobs locations and distance from the player
                // any far mob will need to be repositioned;
                if (!_toStopReposition)
                    RePositionMob();

            }
        }
        public IEntity GetEntity(EntityTagSO entityTagSO)
        {
            for (int i = 0; i < _activeEntities.Count; i++)
            {
                if (_activeEntities[i].EntityTagSO == entityTagSO)
                    return _activeEntities[i];

            }
            throw new System.Exception("Entity was not found active - " + entityTagSO.name);

        }
        private void RePositionMob()
        {
            List<IEntity> tooFarEntities = new List<IEntity>();
            Vector3 playerPosition = _aimAssists.PointOnWorld;
            EntityTagSO entityTagSO = _player.Entity.EntityTagSO;
            var currentTarget = _player.AimAssists.Target;
            for (int i = 0; i < AllEntities.Count; i++)
            {
                IEntity entity = AllEntities[i];
                Vector3 currentPosition = entity.CurrentPosition;
                if (_player.AimAssists.HasTarget)
                {
                    bool isCurrentTarget = currentTarget.InstanceID == entity.InstanceID;
                    if (isCurrentTarget)
                        continue;
                }

                if (Vector3.Distance(currentPosition, playerPosition) > _spawnConfig.ReturnRadius && entity.EntityTagSO != entityTagSO)
                    tooFarEntities.Add(entity);
            }

            if (tooFarEntities.Count > 0)
            {
                for (int i = 0; i < tooFarEntities.Count; i++)
                {
                    IEntity entity = tooFarEntities[i];
                    entity.DestroyHandler.Destroy();
                    entity.Transform.gameObject.SetActive(false);
                    entity.Transform.SetParent(transform);
                }
                tooFarEntities.Clear();
            }
        }

        public void SpawnEntity()
        {
            EntityTagSO mobTag = _spawnConfig.ConductMob(AllEntities);
            SpawnEntity(mobTag);
        }

        public IEntity SpawnEntity(EntityTagSO mobTag, Vector3 spawnLocation)
        {
            IEntity mob = _poolManager.Pull(mobTag);
            Transform t = mob.Transform;
            t.SetParent(null);
            mob.Rigidbody.isKinematic = true;
            t.position = spawnLocation;
            t.gameObject.SetActive(true);
            mob.Rigidbody.isKinematic = false;
            return mob;
        }
        public IEntity SpawnEntity(EntityTagSO mobTag)
        {
            IEntity mob = _poolManager.Pull(mobTag);
            Transform t = mob.Transform;
            t.SetParent(null);
            mob.Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            mob.Rigidbody.isKinematic = true;
            float yPos = _heightConfigSO.GetHeight(mobTag.StartingHeight).Height + _spawningHeightOffset;
            t.position = GenerateSpawnLocation(yPos, _spawnConfig.SpawnRadius);
            t.gameObject.SetActive(true);
            mob.Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            mob.Rigidbody.isKinematic = false;
            return mob;
        }

        public IEntity SpawnEntity(EntityTagSO mobTag, float radius)
        {
            IEntity mob = _poolManager.Pull(mobTag);
            Transform t = mob.Transform;
            t.SetParent(null);
            mob.Rigidbody.isKinematic = true;
            float yPos = _heightConfigSO.GetHeight(mobTag.StartingHeight).Height + _spawningHeightOffset;
            t.position = GenerateSpawnLocation(yPos, radius);
            t.gameObject.SetActive(true);
            mob.Rigidbody.isKinematic = false;
            return mob;
        }
        private Vector3 GenerateSpawnLocation(float yPos, float radius)
        {
            // Vector3 playerPosition = _playerEntity.Transform.position;
            Vector3 playerPosition = _aimAssists.PointOnWorld;
            Vector3 destination = playerPosition;
            destination.y = yPos;
            do
            {
                destination.x = GetRandomPoint(playerPosition.x);
                destination.z = GetRandomPoint(playerPosition.z);

            } while (PointInCameraView(destination));

            Ray rei = new Ray(destination, Vector3.down);
            if (Physics.Raycast(rei, out RaycastHit raycastHit, 500f, -1))
            {
                const float GROUND_OFFSET = 5f;
                destination.y = raycastHit.point.y + GROUND_OFFSET;
            }
            else
            {
                Debug.LogError(System.DateTime.Now.ToString() + "" + "Spawner: Didnt hit the ground?");
                return GenerateSpawnLocation(yPos, radius);
            }

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
            => a > _minViewPort && a < _maxViewPort;

            float GetRandomPoint(float playerAxisPoint)
            {
                float min = playerAxisPoint - radius;
                float max = playerAxisPoint + radius;

                if (min <= max)
                    return UnityEngine.Random.Range(min, max);

                else
                    return UnityEngine.Random.Range(max, min);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _spawnConfig.ReturnRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _spawnConfig.SpawnRadius);

        }


        private void StartGame()
        {
            _startSpawning = true;
        }
    }

}