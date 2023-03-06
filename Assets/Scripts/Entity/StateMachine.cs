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
        public bool LockStateMachine { get; set; }

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
            if (stateType == CurrentStateType || LockStateMachine)
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
        Roam,
        RunAway,
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

        public static event Action OnTargetHit;
        private readonly ILocomotion _movementHandler;
        private readonly IPlayer _player;
        public override StateType StateType => StateType.Dive;
        private readonly IStat _diveSpeed, _movementSpeed;

        private const float minDistanceToTargetAquire= 6f;
        private const float minDistanceToStartCatchingAnimation = 15f;
        bool _isPlayingAnimation;
        public PlayerDiveState(IPlayer player, IStat speed) : base(player.Entity)
        {
            _player = player;
            _diveSpeed = speed;
            
            _movementSpeed = _entity.StatHandler[StatType.MovementSpeed];
            _movementHandler = _entity.MovementHandler;
        }

        public override void OnStateEnter()
        {
            _player.TargetHandler.LockAtTarget();
            _player.CameraManager.ChangeState(CameraState.FaceTowardsEnemy);
            _entity.HeightHandler.ChangeState(HeightType.Animal);
            _movementSpeed.Value += _diveSpeed.Value;
            _isPlayingAnimation = false;
            _movementHandler.StopMovement = true;
            base.OnStateEnter();
        }

        public override void OnStateExit()
        {
            _movementSpeed.Value -= _diveSpeed.Value;
            _movementHandler.StopMovement = false;
            base.OnStateExit();
            _player.AimAssists.UnLockTarget();
        }
        public override void OnStateTick()
        {
            base.OnStateTick();
            AimAssists aimAssists = _player.AimAssists;
            bool hasTarget = aimAssists.HasTarget;

            if (hasTarget)
            {
                Vector3 dir = aimAssists.TargetDirection;
                _movementHandler.MoveTowards(dir);


                Vector3 targetPos = aimAssists.Target.CurrentPosition;
                Vector3 myPos = _entity.CurrentPosition;
                //myPos.y = 0;
                //targetPos.y = 0;
                float distance = Vector3.Distance(targetPos, myPos);
                if (distance <= minDistanceToTargetAquire)
                    OnTargetHit?.Invoke();
                else if (!_isPlayingAnimation && distance <= minDistanceToStartCatchingAnimation)
                {
                    _isPlayingAnimation = true;
                 //   Debug.Log(distance);
                    _entity.VisualHandler.AnimatorController.SetTrigger("Attack");
                }
            }

        }
   
    }

    public class PlayerRecoverState : BaseState
    {
        public static event Action OnRecoverStateTryingToExit;
        private readonly HeightConfigSO _heightConfigSO;
        private readonly IStat _recoverSpeed;
        private readonly IPlayer _player;
        public override StateType StateType => StateType.Recover;
        public PlayerRecoverState(IPlayer player,IStat additionSpeed) : base(player.Entity)
        {
            _player = player;
            _heightConfigSO = _entity.HeightHandler.HeightConfigSO;
            _recoverSpeed = additionSpeed;
        }

        public override void OnStateEnter()
        {
            _entity.HeightHandler.ChangeState(HeightType.Player);
            _player.CameraManager.ChangeState(CameraState.FaceUp);
            _entity.StatHandler[StatType.MovementSpeed].Value += _recoverSpeed.Value;
      
            base.OnStateEnter();
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            _entity.StatHandler[StatType.MovementSpeed].Value -= _recoverSpeed.Value;
        }

        public override void OnStateTick()
        {
     
            CheckHeight();
            base.OnStateTick();
        }

     

        private void CheckHeight()
        {
            float offset = 0.1f;
            float requiredHeight = _heightConfigSO.PlayerHeight.Height;
            Vector3 height = _entity.CurrentPosition;
            height.y = requiredHeight;

            if (Vector3.Distance(_entity.CurrentPosition, height) < offset)
            {
                OnRecoverStateTryingToExit?.Invoke();
                _player.StateMachine.ChangeState(StateType.Idle);
            }
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
     

            _entity.VisualHandler.AnimatorController.SetFloat("Forward", 0);
        }
 
    }

    public class AIMoveState : BaseState
    {

        protected readonly NavMeshAgent _agent;
        protected readonly AIBehaviour Aibehaviour;
        protected readonly NavMeshPath _path;
        public override StateType StateType => StateType.Roam;
        public Vector3 CurrentPos;

        public AIMoveState(AIBehaviour aibehaviour, NavMeshAgent agent) : base(aibehaviour.Entity)
        {
            Aibehaviour = aibehaviour;
            _agent = agent;
            _path = new NavMeshPath();
            _agent.autoRepath = true;
        }

        private void ResetParams()
        {
            Aibehaviour.StartCoroutine(GenerateRandomPoint());
            if (_agent.isActiveAndEnabled && Aibehaviour.gameObject.activeSelf)
            {
            
                _agent.isStopped = false; 
            }
        }

        private  IEnumerator GenerateRandomPoint()
        {
             int attempt = -1;
            const int maxAttemptyPerFrame = 3;
            System.Random rnd = new System.Random();
            do
            {
                if(_agent.isActiveAndEnabled == false)
                    yield break;
                else if (attempt % maxAttemptyPerFrame == 0)
                    yield return null;
                else
                    attempt++;

                Vector3 currentPos = Aibehaviour.CurrentPosition;
                currentPos.x += RND();
                currentPos.z += RND();
                CurrentPos = currentPos;


            } while (!NavMesh.CalculatePath(Aibehaviour.CurrentPosition, CurrentPos, -1, _path) || _path.status != NavMeshPathStatus.PathComplete);
            if (!_agent.isActiveAndEnabled)
                yield break;
            _agent.SetPath(_path);

            _entity.VisualHandler.AnimatorController.SetFloat("Forward", .5f);
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

    public class AIRunAwayState : AIMoveState
    {
        private readonly IStat _runAwaySpeed;
        private readonly IStat _movementSpeed;
        public override StateType StateType => StateType.RunAway;
        public AIRunAwayState(AIBehaviour aibehaviour, NavMeshAgent agent) : base(aibehaviour,agent)
        {
            _runAwaySpeed =  _entity.StatHandler[StatType.RunAwaySpeed];
            _movementSpeed = _entity.StatHandler[StatType.MovementSpeed];
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Debug.Log($"{Aibehaviour.gameObject.name} is Running!");
            _movementSpeed.Value += _runAwaySpeed.Value;
       
        }

        public override void OnStateExit()
        {
            _movementSpeed.Value -= _runAwaySpeed.Value;
            Debug.Log($"{Aibehaviour.gameObject.name} is stopped running!");
            base.OnStateExit();
        }

    }
}