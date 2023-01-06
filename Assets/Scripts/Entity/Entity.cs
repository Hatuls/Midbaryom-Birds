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
        IStatHandler StatHandler { get; }
        ILocomotion MovementHandler { get; }
        IDestroyHandler DestroyHandler { get; }
    }

    public interface IBehaviour
    {
        void ApplyBehaviour();
    }





    public interface IInputReader<T> : IInputReader
    {
        event Action<T> OnInputValueReceived;

    }
    public interface IInputReader : ITaggable
    {
        event Action OnInputReceived;
        event Action OnInputStopped;
    }
    public interface IInputHandler
    {
        IReadOnlyList<IInputReader<float>> TurnningInputs { get; }
        // IReadOnlyList<IInputReader>
    }

    public class Entity : MonoBehaviour, IEntity
    {

        [SerializeField]
        private EntityTagSO _entityTag;
        [SerializeField]
        private StatSO[] _stats;
        [SerializeField]
        private TagSO[] _entityTags;

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
        public EntityTagSO EntityTagSO => _entityTag;

        private void Awake()
        {
            _destroyHandler = new DestroyBehaviour(this);
            _statHandler = new StatHandler(_stats);
            _movementHandler = new Locomotion(transform, false, _statHandler[StatType.MovementSpeed]);
        }
    
        private void OnDestroy()
        {
            _destroyHandler.Destroy();
        }
    }




}