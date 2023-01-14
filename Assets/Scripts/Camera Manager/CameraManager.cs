using Midbaryom.Core;
using System.Collections.Generic;
using UnityEngine;
namespace Midbaryom.Camera
{
    public class CameraManager : IUpdateable
    {
        public readonly Transform CameraTransform;
        public readonly UnityEngine.Camera Camera;
        public readonly IRotator Rotator;
        //private readonly IPlayer _player;
        public CameraManager(IPlayer player,UnityEngine.Camera camera , Transform cameraTransform)
        {
            Camera = camera ;
            CameraTransform = cameraTransform;
            _cameraStates = new Dictionary<CameraState, IState>()
            {
                { CameraState.Default, new DefaultCameraState(this) },
            };

            _currentState = CameraState.Default;
          //  Rotator = new CameraRotator(); //(CameraTransform, false, player.Entity.StatHandler[StatType.RotationSpeed], CameraTransform.rotation);
        }

        private Dictionary<CameraState, IState> _cameraStates;
        private CameraState _currentState;

        private IState CurrentState => GetState(_currentState);
  
        //public CameraRotator(Transform transform, bool toLockRotation, IStat rotationSpeed, Quaternion startRotation) : base(transform, toLockRotation, rotationSpeed, startRotation)
        //{

        //}
        public void ChangeState(CameraState cameraState)
        {
            if (_currentState == cameraState)
                return;

            CurrentState.OnStateExit();

            _currentState = cameraState;

            CurrentState.OnStateEnter();
        }
        private IState GetState(CameraState cameraState)
        {
            if (_cameraStates.TryGetValue(cameraState, out IState state))
                return state;
            throw new System.Exception($"State was not found : {cameraState.ToString()}");
        }
        public void Tick()
        {
            CurrentState.OnStateTick();
        }
    }

 

    public abstract class BaseRotationState : Rotator,IState
    {
        protected readonly CameraManager _cameraRotator;

        public BaseRotationState(CameraManager cameraManager, Transform transform, bool toLockRotation, IStat rotationSpeed, Quaternion startRotation) :base(transform, toLockRotation, rotationSpeed, startRotation)
        {
            _cameraRotator = cameraManager;
        }

        public abstract void OnStateEnter();
        public abstract void OnStateExit();
        public abstract void OnStateTick();


    }

    public class DefaultCameraState : BaseRotationState
    {

        public readonly Vector3 ForwardDirection;
        public DefaultCameraState(CameraManager cameraManager, Transform transform, bool toLockRotation, IStat rotationSpeed, Quaternion startRotation) : base(cameraManager, transform, toLockRotation, rotationSpeed, startRotation)
        {
            ForwardDirection = transform.forward;

        }

        public override void OnStateEnter()
        {
            AssignRotation(StartingRotation() * Vector3.forward);
        }

        public override void OnStateExit()
        {
          
        }

        public override void OnStateTick()
        {
            Tick();
        }

        protected override void RotateTowards()
        {
            Quaternion lookRotation = Quaternion.LookRotation(NewDirection);
            Quaternion lerpDirection = Quaternion.Lerp(_transform.localRotation, lookRotation, RotationSpeed * Time.deltaTime);
            float yAxisRotation = lerpDirection.eulerAngles.y;
            Quaternion YRotation = Quaternion.Euler(0f, yAxisRotation, 0f);
            _transform.localRotation = YRotation;
        }
    }
    public enum CameraState
    {
        Default,
        Hunt,
    }

}