using Midbaryom.Core;
using System.Collections.Generic;
using UnityEngine;
namespace Midbaryom.Camera
{
    public class CameraManager : IUpdateable
    {
        public readonly Transform CameraTransform;
        public readonly UnityEngine.Camera Camera;

        private Dictionary<CameraState, IState> _cameraStates;
        private CameraState _currentState;

        //private readonly IPlayer _player;
        public CameraManager(IPlayer player, UnityEngine.Camera camera, Transform cameraTransform)
        {
            Camera = camera;
            CameraTransform = cameraTransform;
            var stat = player.Entity.StatHandler[StatType.RotationSpeed];

            CameraRotationSO _defaultCameraRotation = GameManager.Instance.HuntUp ;
            CameraRotationSO _huntDownCameraRotation = GameManager.Instance.HuntDown;


            _cameraStates = new Dictionary<CameraState, IState>()
            {
                { CameraState.Default, new DefaultCameraState(_defaultCameraRotation,player.Entity.Transform,CameraTransform,false,stat, CameraTransform.rotation) },
                { CameraState.FaceDown, new HuntCameraState(_huntDownCameraRotation,player.Entity.Transform,CameraTransform,false,stat, CameraTransform.rotation) },
                { CameraState.FaceUp, new HuntCameraState(_defaultCameraRotation,player.Entity.Transform,CameraTransform,false,stat, CameraTransform.rotation) },
            };

            _currentState = CameraState.Default;
            CurrentState.OnStateEnter();
            //  Rotator = new CameraRotator(); //(CameraTransform, false, player.Entity.StatHandler[StatType.RotationSpeed], CameraTransform.rotation);
        }



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



    public abstract class BaseRotationState : Rotator, IState
    {
        protected readonly CameraRotationSO _cameraRotationSO;

        public BaseRotationState(CameraRotationSO cameraConfig, Transform transform, bool toLockRotation, IStat rotationSpeed, Quaternion startRotation) : base(transform, toLockRotation, rotationSpeed, startRotation)
        {
            _cameraRotationSO = cameraConfig;
        }

        public virtual void OnStateEnter() { }
        public virtual void OnStateExit() { }
        public virtual void OnStateTick() => Tick();




    }

    public class DefaultCameraState : BaseRotationState
    {

        private readonly Transform _objectTransform;

        public DefaultCameraState(CameraRotationSO cameraRotationSO, Transform ObjectTransform, Transform transform, bool toLockRotation, IStat rotationSpeed, Quaternion startRotation) : base(cameraRotationSO, transform, toLockRotation, rotationSpeed, startRotation)
        {
            _objectTransform = ObjectTransform;
        }



        protected override void RotateTowards()
        {
            _direction = Quaternion.AngleAxis(_cameraRotationSO.Angle, Vector3.right) * _objectTransform.forward;

            Quaternion lookRotation = Quaternion.LookRotation(NewDirection);
            Quaternion lerpDirection = Quaternion.Lerp(_transform.localRotation, lookRotation, RotationSpeed * Time.deltaTime);

            _transform.localRotation = lerpDirection;
        }
    }

    public class HuntCameraState : BaseRotationState
    {
        private readonly Transform _objectTransform;
        private float _currentAngle;
        private float _currentTime;
        private float _startingAngle;
        private Quaternion _rotation;
        public HuntCameraState( CameraRotationSO cameraRotationSO, Transform ObjectTransform, Transform transform, bool toLockRotation, IStat rotationSpeed, Quaternion startRotation) : base(cameraRotationSO, transform, toLockRotation, rotationSpeed, startRotation)
        {
   
            _objectTransform = ObjectTransform;
        }

        public override void OnStateEnter()
        {
            _rotation = _transform.localRotation;
            _startingAngle = _transform.localEulerAngles.x; 
            _currentAngle = 0;
            _currentTime = 0;
        }
        public override void OnStateTick()
        {
            CalculateAngle();
            base.OnStateTick();
        }

        private void CalculateAngle()
        {
            // maybe change it to currentvalue

          //  Debug.Log($"");
            float remain = _cameraRotationSO.Angle - _startingAngle;
            _currentAngle = _startingAngle + _cameraRotationSO.Evaluate(_currentTime ) * remain;
            _currentTime += Time.deltaTime;

            AssignRotation(Quaternion.AngleAxis(_currentAngle, _objectTransform.right) * _objectTransform.forward);
        }
        //protected override bool CanRotate()
        //=> !_lockRotation;
        protected override void RotateTowards()
        {
            Quaternion lookRotation = Quaternion.LookRotation(NewDirection);
            Quaternion lerpDirection = Quaternion.Lerp(_rotation, lookRotation, _currentTime / _cameraRotationSO.Duration);
            lerpDirection.eulerAngles = new Vector3(lerpDirection.eulerAngles.x, 0f, 0f);
            _transform.localRotation = lerpDirection;
        }
    }



    public enum CameraState
    {
        Default,
        FaceDown,
        FaceUp,
    } 
}

