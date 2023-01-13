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
        EntityTagSO EntityTagSO { get; }
        IRotator Rotator { get; }
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

        public Vector3 CurrentFacingDirection => MovementHandler.CurrentFacingDirection;
        public Vector3 CurrentPosition => MovementHandler.CurrentPosition;

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

        private void Awake()
        {
            _destroyHandler = new DestroyBehaviour(this);
            _statHandler = new StatHandler(_stats);
            _rotator = new Rotator(transform, _rigidbody,_statHandler[StatType.RotationSpeed]);
            _movementHandler = new Locomotion(transform, _rigidbody, false, _statHandler[StatType.MovementSpeed]);
        }
        private void Update()
        {
            _rotator.RotationTick();
        }
        private void FixedUpdate()
        {
           
            _movementHandler.FixedUpdateTick();
        }
        private void OnDestroy()
        {
            _destroyHandler.Destroy();
        }
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