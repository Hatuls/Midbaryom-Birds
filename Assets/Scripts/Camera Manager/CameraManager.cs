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
            var rotationSpeed = player.Entity.StatHandler[StatType.RotationSpeed];
            var movementSpeed = player.Entity.StatHandler[StatType.MovementSpeed];
            CameraRotationSO _defaultCameraRotation = GameManager.Instance.HuntUp;
            CameraRotationSO _huntDownCameraRotation = GameManager.Instance.HuntDown;


            _cameraStates = new Dictionary<CameraState, IState>()
            {
                { CameraState.Default, new DefaultCameraState(_defaultCameraRotation,player.Entity.Transform,CameraTransform,false,rotationSpeed, CameraTransform.rotation) },
                { CameraState.FaceTowardsEnemy, new HuntDiveInCameraState(_huntDownCameraRotation,player.Entity.Transform,CameraTransform,false,rotationSpeed, CameraTransform.rotation,player.AimAssists,movementSpeed,player.Entity.Rotator)  },
                { CameraState.FaceUp, new HuntCameraState(_defaultCameraRotation,player.Entity.Transform,CameraTransform,false,rotationSpeed, CameraTransform.rotation) },
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
        public event System.Action OnStateEnterEvent;
        public event System.Action OnStateExitEvent;
        public event System.Action OnStateTickEvent;

        public BaseRotationState(CameraRotationSO cameraConfig, Transform transform, bool toLockRotation, IStat rotationSpeed, Quaternion startRotation) : base(transform, toLockRotation, rotationSpeed, startRotation)
        {
            _cameraRotationSO = cameraConfig;
        }





        public virtual void OnStateEnter() { OnStateEnterEvent?.Invoke(); }
        public virtual void OnStateExit() { OnStateExitEvent?.Invoke(); }
        public virtual void OnStateTick() { Tick(); OnStateTickEvent?.Invoke(); }


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
    public class HuntDiveInCameraState : HuntCameraState
    {
        private readonly AimAssists _aimAssists;
        private readonly IStat _speedStat;
        private readonly IRotator _rotator;
        public HuntDiveInCameraState(CameraRotationSO cameraRotationSO, Transform ObjectTransform, Transform transform, bool toLockRotation, IStat rotationSpeed, Quaternion startRotation, AimAssists aimAssists, IStat speed, IRotator rotator) : base(cameraRotationSO, ObjectTransform, transform, toLockRotation, rotationSpeed, startRotation)
        {
            _aimAssists = aimAssists;
            _speedStat = speed;
            _rotator = rotator;
        }
    
        protected override void CalculateAngle()
        {
            if (_aimAssists.HasTarget)
            {
                Vector3 myPos = _objectTransform.position;
                Vector3 targetPos = _aimAssists.Target.CurrentPosition;
                float distance = Vector3.Distance(myPos, targetPos);
                float divition = _objectTransform.position.y / distance;
                float angle = Mathf.Sin(divition) * Mathf.Rad2Deg;

                _currentAngle = _startingAngle + angle * _cameraRotationSO.Evaluate(_currentTime);
                Vector3 v = Quaternion.AngleAxis(_currentAngle, _objectTransform.right) * _objectTransform.forward;

                AssignRotation(v);

                _currentTime += Time.deltaTime;
            }
        }

        protected override void RotateTowards()
        {
            Vector3 myPos = _objectTransform.position;
            Vector3 targetPos = _aimAssists.Target.CurrentPosition;
            float distance = Vector3.Distance(myPos, targetPos);
            float time = distance / _speedStat.Value;
            Quaternion lookRotation = Quaternion.LookRotation(NewDirection);
            Quaternion lerpDirection = Quaternion.Lerp(_rotation, lookRotation, _currentTime / time);
            lerpDirection.eulerAngles = new Vector3(lerpDirection.eulerAngles.x, 0f, 0f);
            _transform.localRotation = lerpDirection;
        }
    }
    public class HuntCameraState : BaseRotationState
    {
        protected readonly Transform _objectTransform;

        protected float _currentAngle;
        protected float _currentTime;
        protected float _startingAngle;
        protected Quaternion _rotation;
        public HuntCameraState(CameraRotationSO cameraRotationSO, Transform ObjectTransform, Transform transform, bool toLockRotation, IStat rotationSpeed, Quaternion startRotation) : base(cameraRotationSO, transform, toLockRotation, rotationSpeed, startRotation)
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

        protected virtual void CalculateAngle()
        {
            // maybe change it to currentvalue

            float remain = _cameraRotationSO.Angle - _startingAngle;
            _currentAngle = _startingAngle + _cameraRotationSO.Evaluate(_currentTime) * remain;
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
        FaceTowardsEnemy,
        FaceUp,
    }
}

