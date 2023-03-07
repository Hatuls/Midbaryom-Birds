using Midbaryom.Camera;
using Midbaryom.Visual;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Midbaryom.Core
{
    /// <summary>
    /// Any Entity in the game
    /// </summary>
    public interface IEntity : ITrackable, ITaggable
    {
        ITargetBehaviour TargetBehaviour { get; }
        EntityTagSO EntityTagSO { get; }
        Rigidbody Rigidbody { get; }
        IRotator Rotator { get; }
        IHeightHandler HeightHandler { get; set; }
        IStatHandler StatHandler { get; }
        ILocomotion MovementHandler { get; }
        IDestroyHandler DestroyHandler { get; }
        IVisualHandler VisualHandler { get; }
    }

    public interface IBehaviour
    {
        void ApplyBehaviour();
    }


    public class Entity : MonoBehaviour, IEntity
    {
        [Header("Tags:")]
        [SerializeField]
        private EntityTagSO _entityTag;
        [SerializeField]
        private StatSO[] _stats;

        [Header("Components:")]
        [SerializeField]
        private Transform _transform;
        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private VisualHandler _visualHandler;
        [SerializeField]
        private AnimationCurve _rotationCurve;
        [SerializeField]
        private TargetedBehaviour _targetBehaviour;
        private IRotator _rotator;
        private IDestroyHandler _destroyHandler;
        private IStatHandler _statHandler;
        private ILocomotion _movementHandler;
        private IHeightHandler _heightHandler;
        public Vector3 CurrentFacingDirection => MovementHandler.CurrentFacingDirection;
        public Vector3 CurrentPosition => MovementHandler.CurrentPosition;
        public Transform Transform => _transform;
        public IVisualHandler VisualHandler => _visualHandler;
        public IEnumerable<TagSO> Tags
        {
            get
            {
                yield return _entityTag;
                
                foreach (TagSO tag in _entityTag.Tags)
                    yield return tag;
            }
        }
        public ILocomotion MovementHandler => _movementHandler;
        public IDestroyHandler DestroyHandler => _destroyHandler;
        public IStatHandler StatHandler => _statHandler;
        public IRotator Rotator => _rotator;
        public EntityTagSO EntityTagSO => _entityTag;
        public ITargetBehaviour TargetBehaviour => _targetBehaviour;
        public Rigidbody Rigidbody => _rigidbody;
        public IHeightHandler HeightHandler { get => _heightHandler; set => _heightHandler = value; }

        public IEnumerable<IUpdateable> UpdateNeeded
        {
            get
            {
                yield return Rotator;
                yield return MovementHandler;
                yield return HeightHandler;
            }
        }

        private void Awake()
        {
            _destroyHandler = new DestroyBehaviour(this);
            _statHandler = new StatHandler(_stats);
            _rotator = new Rotator(_transform, false, _statHandler[StatType.RotationSpeed],_transform.rotation, _rotationCurve);
            _movementHandler = new Locomotion(_transform, Rigidbody, false, _statHandler[StatType.MovementSpeed]);
            _heightHandler = new HeightHandler(MovementHandler as Locomotion, Transform, EntityTagSO.StartingHeight);

        }
        private void OnEnable()
        {
            Spawner.RegisterEntity(this);
            VisualHandler?.Init(this);
        }
      
        private void Update()
        {
            foreach (IUpdateable updateable in UpdateNeeded)
                updateable.Tick();
        }

        private void OnDisable()
        {
            Spawner.RemoveEntity(this);
        }
        private void OnDestroy()
        {
            _destroyHandler.Destroy();
        }
    }
    public interface IUpdateable
    {
        void Tick();
    }
    public interface IState
    {
        event Action OnStateEnterEvent,OnStateExitEvent,OnStateTickEvent;
        void OnStateEnter();
        void OnStateExit();
        void OnStateTick();
    }

}