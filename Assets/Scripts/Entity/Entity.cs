using Midbaryom.Camera;
using System.Collections.Generic;
using UnityEngine;

namespace Midbaryom.Core
{
    /// <summary>
    /// Any Entity in the game
    /// </summary>
    public interface IEntity : ITrackable, ITaggable
    {
        EntityTagSO EntityTagSO { get; }
        Transform Transform { get; }
        IRotator Rotator { get; }
        IHeightHandler HeightHandler { get; }
        IStatHandler StatHandler { get; }
        ILocomotion MovementHandler { get; }
        IDestroyHandler DestroyHandler { get; }
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
        [SerializeField]
        private TagSO[] _entityTags;
        [Header("Components:")]
        [SerializeField]
        private Rigidbody _rigidbody;


        private IRotator _rotator;
        private IDestroyHandler _destroyHandler;
        private IStatHandler _statHandler;
        private ILocomotion _movementHandler;
        private IHeightHandler _heightHandler;
        public Vector3 CurrentFacingDirection => MovementHandler.CurrentFacingDirection;
        public Vector3 CurrentPosition => MovementHandler.CurrentPosition;
        public Transform Transform => transform;

        public IEnumerable<TagSO> Tags
        {
            get
            {
                yield return _entityTag;

                for (int i = 0; i < _entityTags.Length; i++)
                    yield return _entityTags[i];
            }
        }
        public ILocomotion MovementHandler => _movementHandler;
        public IDestroyHandler DestroyHandler => _destroyHandler;
        public IStatHandler StatHandler => _statHandler;
        public IRotator Rotator => _rotator;
        public EntityTagSO EntityTagSO => _entityTag;

        public IHeightHandler HeightHandler => _heightHandler;

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
            _rotator = new Rotator(transform, false, _statHandler[StatType.RotationSpeed],transform.rotation);
            _movementHandler = new Locomotion(transform, _rigidbody, false, _statHandler[StatType.MovementSpeed]);

        }
        private void Start()
        {
            _heightHandler = new HeightHandler(_movementHandler as Locomotion,Transform, _entityTag.StartingHeight);
        }
        private void Update()
        {
            foreach (IUpdateable updateable in UpdateNeeded)
                updateable.Tick();
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
        void OnStateEnter();
        void OnStateExit();
        void OnStateTick();
    }
    //public interface IInputReader<T> : IInputReader
    //{
    //    event Action<T> OnInputValueReceived;

    //}
    //public interface IInputReader : ITaggable
    //{
    //    event Action OnInputReceived;
    //    event Action OnInputStopped;
    //}
    //public interface IInputHandler
    //{
    //    IReadOnlyList<IInputReader<float>> TurnningInputs { get; }
    //    // IReadOnlyList<IInputReader>
    //}
}