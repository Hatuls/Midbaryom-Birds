using Midbaryom.AI;
using Midbaryom.Camera;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Midbaryom.Core
{
    public class StateMachine : IStateMachine
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

            GetState(_currentStateType).OnStateEnter();
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

        private readonly AIBehaviour _aIBehaviour;
        private readonly NavMeshAgent agent;

        public AIIdleState(AIBehaviour aIBehaviour, NavMeshAgent agent) : base(aIBehaviour.Entity)
        {
            _aIBehaviour = aIBehaviour;
            this.agent = agent;
        }
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            var go = _aIBehaviour.gameObject;
            if (go.activeSelf&&go.activeInHierarchy && agent.isActiveAndEnabled)
                agent.isStopped = true;
     

            _entity.VisualHandler.AnimatorController.Animator.SetFloat("Forward", 0);
        }
 
    }

    public class AIMoveState : BaseState
    {

        private readonly NavMeshAgent _agent;
        private readonly AIBehaviour Aibehaviour;
        private readonly NavMeshPath _path;
        public override StateType StateType => StateType.Run;
        public Vector3 CurrentPos;

        public AIMoveState(AIBehaviour aibehaviour, NavMeshAgent agent) : base(aibehaviour.Entity)
        {
            Aibehaviour = aibehaviour;
            _agent = agent;
            _path = new NavMeshPath();
            _agent.autoRepath = true;
        }

        private  void ResetParams()
        {
            Aibehaviour.StartCoroutine(GenerateRandomPoint());
            if(Aibehaviour.gameObject.activeSelf && _agent.isActiveAndEnabled)
            _agent.isStopped = false;
        }

        private  IEnumerator GenerateRandomPoint()
        {
             int attempt = -1;
            const int maxAttemptyPerFrame = 3;
            System.Random rnd = new System.Random();
            do
            {
                if (attempt % maxAttemptyPerFrame == 0)
                    yield return null;
                else
                    attempt++;

                Vector3 currentPos = Aibehaviour.CurrentPosition;
                currentPos.x += RND();
                currentPos.z += RND();
                CurrentPos = currentPos;


            } while (!NavMesh.CalculatePath(Aibehaviour.CurrentPosition, CurrentPos, -1, _path) || _path.status != NavMeshPathStatus.PathComplete);
            _agent.SetPath(_path);

            _entity.VisualHandler.AnimatorController.Animator.SetFloat("Forward", .5f);
            float RND()
            {
                int radius = System.Convert.ToInt32(Aibehaviour.AIBehaviourSO.TargetDestinationRadius);
                return rnd.Next(-radius, radius);
            }
        }

        public override void OnStateEnter()
        {
            ResetParams();
   


            base.OnStateExit();
        }


        public override void OnStateExit()
        {
            base.OnStateExit();
            Aibehaviour.StopCoroutine(GenerateRandomPoint());


        }

    }
}