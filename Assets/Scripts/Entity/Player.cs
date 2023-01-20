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

        private StateMachine _playerStateMachine;
        public IEntity Entity => _entity;
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

        private void Start()
        {
            CameraManager = new CameraManager(this, _camera, _cameraTransform);
            PlayerController = new PlayerController(this, Entity);
            InitStateMachine();
        }

        private void InitStateMachine()
        {
            BaseState[] baseStates = new BaseState[]
            {
                new PlayerIdleState(this),
                new PlayerDiveState(this,Entity.StatHandler[StatType.DiveSpeed]),
                new PlayerRecoverState(this),
            };
            _playerStateMachine = new StateMachine(StateType.Idle, baseStates);
        }

        private void Update()
        {
            foreach (IUpdateable updateable in UpdateCollection)
                updateable.Tick();
        }
        private void LateUpdate()
        {
            CameraManager.Tick();
        }

        //[ContextMenu("HuntDown")]
        //public void HuntDown()
        //{
        //    CameraManager.ChangeState(CameraState.FaceDown);
        //    Entity.HeightHandler.ChangeState(HeightType.Animal);
        //}
        //[ContextMenu("HuntUp")]
        //public void HuntUp()
        //{
        //    Entity.HeightHandler.ChangeState(HeightType.Player);
        //    CameraManager.ChangeState(CameraState.FaceUp);
        //}
    }

    public interface IPlayer : ITaggable
    {
        IEntity Entity { get; }
        IStateMachine StateMachine { get; }
         PlayerController PlayerController { get; }
        CameraManager CameraManager { get; }
    }




   
}