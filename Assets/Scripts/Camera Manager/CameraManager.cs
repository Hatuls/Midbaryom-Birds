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

      public bool IsActive { get; set; }
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
                { CameraState.LookAtCarcass, new LookAtCarcassState(_huntDownCameraRotation, player.TargetHandler, cameraTransform, player.Entity.Transform, false, rotationSpeed, cameraTransform.rotation) }

            };
            IsActive = true;
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
            if (_currentState == cameraState || !IsActive)
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
            if(IsActive)
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
    public class LookAtCarcassState : BaseRotationState
    {
        private readonly Transform _cameraHead;
        private Quaternion _startingRotation;
        private Quaternion _finalRotation;
        private float _duration = 0.25f;
        private float _lockToTargetCounter = 0;
        public LookAtCarcassState(CameraRotationSO cameraConfig,BaseTargetHandler targetHandler,Transform objectTransform, Transform transform, bool toLockRotation, IStat rotationSpeed, Quaternion startRotation) : base(cameraConfig, transform, toLockRotation, rotationSpeed, startRotation)
        {
     
            _cameraHead = objectTransform;
            TargetHandler = targetHandler;
            _finalRotation = new Quaternion();
            _finalRotation.eulerAngles = new Vector3(90f, 0, 0);
        }

        public BaseTargetHandler TargetHandler { get; }
        public override void OnStateEnter()
        {
            _lockToTargetCounter = 0;
            StopRotation = true;
            _startingRotation = _cameraHead.localRotation;
            base.OnStateEnter();
        }
        protected override void RotateTowards()
        {
            if (TargetHandler.HasTargetAttached)
            {
                _lockToTargetCounter += Time.deltaTime;
                Quaternion q = Quaternion.Lerp(_startingRotation, _finalRotation, _lockToTargetCounter / _duration);

                _cameraHead.localRotation = q;
            }
        }
        public override void OnStateExit()
        {
            StopRotation = false;
            base.OnStateExit(); 
        }
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
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _startingAngle = _transform.localEulerAngles.x;
        }
        protected override void CalculateAngle()
        {
            if (_aimAssists.HasTarget)
            {
                Vector3 myPos = _objectTransform.position;
                Vector3 targetPos = _aimAssists.Target.CurrentPosition;
                Vector3 dir = targetPos - myPos;
                dir.Normalize();
        
                Vector3 v = Vector3.Lerp(_transform.forward, dir, _currentTime / _cameraRotationSO.Duration);
                _transform.rotation = Quaternion.LookRotation(v, Vector3.up);


                _currentTime += Time.deltaTime;
            }
        }

        protected override void RotateTowards()
        {
            
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
        LookAtCarcass,
    }
}

