using Midbaryom.Camera;
using Midbaryom.Inputs;
using System.Collections.Generic;
using UnityEngine;

namespace Midbaryom.Core
{
    public class Player : MonoBehaviour, IPlayer
    {
        [SerializeField]
        private Entity _entity;
        [SerializeField]
        private Transform _cameraTransform;
        [SerializeField]
        private UnityEngine.Camera _camera;
        [SerializeField]
        private AimAssists _aimAssists;
        [SerializeField]
        private TargetHandler _targetHandler;

        [SerializeField]
        private ParticleSystem _particleSystem;
        private PlayerDiveState _playerDiveState;
        private StateMachine _playerStateMachine;
        public IEntity Entity => _entity;
        public AimAssists AimAssists => _aimAssists;
        public IEnumerable<TagSO> Tags => Entity.Tags;
        public PlayerController PlayerController { get; set; }
        public CameraManager CameraManager { get; set; }
        public IEnumerable<IUpdateable> UpdateCollection
        {
            get
            {
                yield return PlayerController;
                yield return _playerStateMachine;
            }
        }

        public IStateMachine StateMachine => _playerStateMachine;

        public TargetHandler TargetHandler => _targetHandler;

        private void Start()
        {
            Entity.HeightHandler = new HeightHandler(Entity.MovementHandler as Locomotion,
                                                     Entity.Transform, 
                                                     Entity.EntityTagSO.StartingHeight,
                                                     this);

            CameraManager = new CameraManager(this, _camera, _cameraTransform);
            PlayerController = new PlayerController(this, Entity);

            InitStateMachine();
         //   _aimAssists.OnTargetReset += ExitState;
        }

        private void InitStateMachine()
        {
            _playerDiveState = new PlayerDiveState(this, Entity.StatHandler[StatType.DiveSpeed]);
            _playerDiveState.OnStateEnterEvent += _particleSystem.Play;
            _playerDiveState.OnStateExitEvent += _particleSystem.Stop;
 
            BaseState[] baseStates = new BaseState[]
            {
                new PlayerIdleState(this),
                _playerDiveState,
                new PlayerRecoverState(this,Entity.StatHandler[StatType.RecoverSpeed]),
            };
            _playerStateMachine = new StateMachine(StateType.Idle, baseStates);
        }
        private void ExitState() => _playerStateMachine.ChangeState(StateType.Recover);
        private void Update()
        {
            foreach (IUpdateable updateable in UpdateCollection)
                updateable.Tick();
        }
        
        private void LateUpdate()
        {
            CameraManager.Tick();
        }
        private void OnDestroy()
        {
            _aimAssists.OnTargetReset -= ExitState;
            _playerDiveState.OnStateEnterEvent -= _particleSystem.Play;
            _playerDiveState.OnStateExitEvent  -= _particleSystem.Stop;
        }
    }

    public interface IPlayer : ITaggable
    {
        IEntity Entity { get; }
        IStateMachine StateMachine { get; }
        PlayerController PlayerController { get; }
        AimAssists AimAssists { get; }
        TargetHandler TargetHandler { get; }
        CameraManager CameraManager { get; }
    }




   
}