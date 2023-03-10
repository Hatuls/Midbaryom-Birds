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
        private BaseTargetHandler _targetHandler;
        [SerializeField]
        private BirdStateMachine _playerStateMachine;

        
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
                yield return CameraManager;
            }
        }

        public IStateMachine StateMachine => _playerStateMachine.StateMachine;

        public BaseTargetHandler TargetHandler => _targetHandler;

        private void Start()
        {
            Entity.HeightHandler = new HeightHandler(Entity.MovementHandler as Locomotion,
                                                     Entity.Transform, 
                                                     Entity.EntityTagSO.StartingHeight,
                                                     this);

            CameraManager = new CameraManager(this, _camera, _cameraTransform);
            PlayerController = new PlayerController(this, Entity);
            _playerStateMachine.InitStateMachine();
            _targetHandler.InitTargetHandler(this);

        }

     
        private void Update()
        {
            foreach (IUpdateable updateable in UpdateCollection)
                updateable.Tick();
        }
        
     
    }


    public interface IPlayer : ITaggable
    {
        IEntity Entity { get; }
        IStateMachine StateMachine { get; }
        PlayerController PlayerController { get; }
        AimAssists AimAssists { get; }
        BaseTargetHandler TargetHandler { get; }
        CameraManager CameraManager { get; }
    }




   
}