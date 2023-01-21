using Midbaryom.Camera;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Midbaryom.Core
{
    public class StateMachine :  IStateMachine
    {
        private StateType _currentStateType;
        protected Dictionary<StateType, IState> _stateDictionary;

        public IReadOnlyDictionary<StateType, IState> StateDictionary => _stateDictionary;
        public StateType CurrentStateType => _currentStateType;
        public IState CurrentState => GetState(_currentStateType);

        public StateMachine(StateType startingState, params BaseState[] baseStates)
        {
            _currentStateType = startingState;
            _stateDictionary = new Dictionary<StateType, IState>();
            for (int i = 0; i < baseStates.Length; i++)
                _stateDictionary.Add(baseStates[i].StateType, baseStates[i]);
        }
        private IState GetState(StateType stateType)
        {
            if (StateDictionary.TryGetValue(stateType, out IState state))
                return state;
            throw new System.Exception("Tried to get a state that is not in the dictionary\nState: " + stateType.ToString());
        }

        public void ChangeState(StateType stateType)
        {
            if (stateType == CurrentStateType)
                return;

            CurrentState.OnStateExit();
            _currentStateType = stateType;
            CurrentState.OnStateEnter();
        }

        public virtual void Tick()
        {
            CurrentState.OnStateTick();
        }
    }

    public enum StateType
    {
        Idle,
        Dive,
        Recover,
        Run,

    }


    public abstract class BaseState : IState
    {
        public event Action OnStateEnterEvent, OnStateExitEvent, OnStateTickEvent;
        protected readonly IEntity _entity;
        public abstract StateType StateType { get; }
        public BaseState(IEntity entity)
        {
            _entity = entity;
        }
        public virtual void OnStateEnter() => OnStateEnterEvent?.Invoke();

        public virtual void OnStateExit() => OnStateExitEvent?.Invoke();

        public virtual void OnStateTick() => OnStateTickEvent?.Invoke();
    }

    public class PlayerIdleState : BaseState
    {
        private readonly IPlayer _player;
        public PlayerIdleState(IPlayer player) : base(player.Entity)
        {
            _player = player;
        }
        public override StateType StateType => StateType.Idle;
     
    }

    public class PlayerDiveState : BaseState
    {
        private readonly IPlayer _player;
        public override StateType StateType => StateType.Dive;
        private readonly IStat _diveSpeed, _movementSpeed;
        public PlayerDiveState(IPlayer player, IStat speed) : base(player.Entity)
        {
            _player = player;
            _diveSpeed = speed;
            _movementSpeed = _entity.StatHandler[StatType.MovementSpeed];
        }

        public override void OnStateEnter()
        {
            _player.CameraManager.ChangeState(CameraState.FaceDown);
            _entity.HeightHandler.ChangeState(HeightType.Animal);
            _movementSpeed.Value += _diveSpeed.Value;
            base.OnStateEnter();
        }

    }

    public class PlayerRecoverState : BaseState
    {
        private readonly HeightConfigSO _heightConfigSO;
       
        private readonly IPlayer _player;
        public override StateType StateType => StateType.Recover;
        public PlayerRecoverState(IPlayer player) : base(player.Entity)
        {
            _player = player;
            _heightConfigSO = _entity.HeightHandler.HeightConfigSO;
        }

        public override void OnStateEnter()
        {
            _entity.HeightHandler.ChangeState(HeightType.Player);
            _player.CameraManager.ChangeState(CameraState.FaceUp);
            _entity.StatHandler[StatType.MovementSpeed].Reset();
            base.OnStateEnter();
        }


        public override void OnStateTick()
        {
            float offset = 0.1f;
            float requiredHeight = _heightConfigSO.PlayerHeight.Height;
            Vector3 height = _entity.CurrentPosition;
            height.y = requiredHeight;

            if (Vector3.Distance(_entity.CurrentPosition, height) < offset)
                _player.StateMachine.ChangeState(StateType.Idle);

            base.OnStateTick();
        }

    }







    public class AIIdleState : BaseState
    {
        public override StateType StateType => StateType.Idle;
        public AIIdleState(IEntity entity) : base(entity)
        {
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _entity.MovementHandler.StopMovement = true;
            _entity.Rotator.StopRotation = true;
            _entity.VisualHandler.AnimatorController.Animator.SetFloat("Forward", 0);
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            _entity.MovementHandler.StopMovement = false;
            _entity.Rotator.StopRotation = false;
 

        }
    }

    public class AIMoveState : BaseState
    {
        private static float _angle = 40f;
        private float _counter;
        private float _duration;
        private Vector2 _rotationTime;
        public override StateType StateType => StateType.Run;
        public AIMoveState(IEntity entity) : base(entity)
        {
            _rotationTime = new Vector2(2f, 3.5f);
        }
        public override void OnStateTick()
        {
            base.OnStateTick();
            _counter += Time.deltaTime;
            if (_counter >= _duration)
                ChangeRotation();
        }

        private void ResetParams()
        {
            _counter = 0;
            _duration = UnityEngine.Random.Range(_rotationTime.x, _rotationTime.y);
        }

        public override void OnStateEnter()
        {
            _entity.VisualHandler.AnimatorController.Animator.SetFloat("Forward", .5f);
            base.OnStateExit();
            ResetParams();
        }

        private void ChangeRotation()
        {
            ResetParams();
            Vector3 dir = Quaternion.AngleAxis(UnityEngine.Random.Range(-_angle, _angle), Vector3.up) * _entity.CurrentFacingDirection;
            _entity.Rotator.AssignRotation(dir);
        }
    }
}