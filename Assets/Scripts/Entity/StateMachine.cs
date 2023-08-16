using Midbaryom.AI;
using Midbaryom.Camera;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Midbaryom.Core
{
    public class StateMachine : MonoBehaviour, IStateMachine
    {
        [SerializeField] private StateType _currentStateType;
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
        Eat,
    }


    public abstract class BaseState : MonoBehaviour, IState
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
        public event Action OnTargetHit;
        public event Action OnCloseToTarget;
        private const float minDistanceToTargetAquire = 5f;
        public const float minDistanceToStartCatchingAnimation = 12f;

        private readonly ILocomotion _movementHandler;
        private readonly IPlayer _player;
        private readonly IStat _diveXZSpeed, _movementSpeed;
        private float _rotatingAngle = .05f;
        private bool _isPlayingAnimation;
        public override StateType StateType => StateType.Dive;
        public PlayerDiveState(IPlayer player, IStat speed) : base(player.Entity)
        {
            _player = player;
            _diveXZSpeed = speed;
            
            _movementSpeed = _entity.StatHandler[StatType.MovementSpeed];
            _movementHandler = _entity.MovementHandler;
        }

        public override void OnStateEnter()
        {
            _movementSpeed.Value += _diveXZSpeed.Value;
            _player.TargetHandler.LockAtTarget();
            _player.CameraManager.ChangeState(CameraState.FaceTowardsEnemy);
            _entity.HeightHandler.ChangeState(HeightType.Animal);
            _isPlayingAnimation = false;
            _movementHandler.StopMovement = true;
            _player.PlayerController.LockInputs = true;
            base.OnStateEnter();

            if(SoundManager.Instance.isSoundPlaying(sounds.WindForToutorial))
            {
                SoundManager.Instance.StopSound(sounds.WindForToutorial);
            }

            if(SoundManager.Instance.isSoundPlaying(sounds.StartFlyingInGame))
            {
                SoundManager.Instance.StopSound(sounds.StartFlyingInGame);
            }

            SoundManager.Instance.CallPlaySound(sounds.tslila);
        }

        public override void OnStateExit()
        {
            _movementSpeed.Value -= _diveXZSpeed.Value;
          //  _entity.Rotator.StopRotation = false;
            _movementHandler.StopMovement = false;
            _player.PlayerController.LockInputs = false;
            base.OnStateExit();

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
                RotateBodyTowards(dir);
                Vector3 targetPos = aimAssists.Target.CurrentPosition;
                Vector3 myPos = _entity.CurrentPosition;
       




                float distance = Vector3.Distance(targetPos, myPos);
                if (distance <= minDistanceToTargetAquire)
                    OnTargetHit?.Invoke();
                else if (!_isPlayingAnimation && distance <= minDistanceToStartCatchingAnimation)
                {
                    _isPlayingAnimation = true;
                    OnCloseToTarget?.Invoke();
                    _entity.VisualHandler.AnimatorController.SetTrigger("Attack");

                    SoundManager.Instance.StopSound(sounds.tslila);
                    SoundManager.Instance.CallPlaySound(sounds.Grab);

                    //_entity.VisualHandler.AnimatorController.SetBool("IsEating", true);

                }
            }

        }
      
        private void RotateBodyTowards(Vector3 dir)
        {
            if (Vector3.Dot(_entity.CurrentFacingDirection, dir) == 1)// no need to rotate if we are facing  the target
                return;

            //we want to know if we are facing towards the target
            // so we get the crossed vector of the 2 directions
            Vector3 right = Vector3.Cross(_entity.CurrentFacingDirection, dir);
            // getting the dot product of the up vector and the crossed vector we received
            float d = Vector3.Dot(right, Vector3.up);

            Vector3 falseInput = Vector3.zero;

            // we only want to change rotation around the X axis 
            if (d > _rotatingAngle)
                falseInput.x = 1f;
            else if (d < -_rotatingAngle)
                falseInput.x = -1;
     
            _entity.Rotator.AssignRotation(falseInput);
        }
    }

    public class PlayerRecoverState : BaseState
    {
        public event Action OnRecoverStateTryingToExit;
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
            SoundManager.Instance.CallPlaySound(sounds.StartFlyingInGame);

            _entity.HeightHandler.ChangeState(HeightType.Player);
            _player.CameraManager.ChangeState(CameraState.FaceUp);
            _entity.StatHandler[StatType.MovementSpeed].Value += _recoverSpeed.Value;
            _entity.Rotator.StopRotation = false;
            base.OnStateEnter();
        }
        public override void OnStateExit()
        {
            _entity.StatHandler[StatType.MovementSpeed].Value -= _recoverSpeed.Value;
            _player.AimAssists.UnLockTarget();
            base.OnStateExit();
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
            if (go.activeSelf&&go.activeInHierarchy && agent.isActiveAndEnabled && agent.isOnNavMesh)
                agent.isStopped = true;
     

            _entity.VisualHandler.AnimatorController.SetFloat("Forward", 0);
        }
 
    }

    public class AIMoveState : BaseState
    {
        private const float HEIGHT_OFFSET = 50f;
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
            if (_agent.isActiveAndEnabled && Aibehaviour.gameObject.activeSelf&& _agent.isOnNavMesh)
            {
            Aibehaviour.StartCoroutine(GenerateRandomPoint());
            
                _agent.isStopped = false; 
            }
        }

        private  IEnumerator GenerateRandomPoint()
        {
             int attempt = -1;
            const int maxAttemptPerFrame = 3;
            System.Random rnd = new System.Random();

            do
            {
                if(_agent.isActiveAndEnabled == false)
                    yield break;
                else if (attempt % maxAttemptPerFrame == 0)
                    yield return null;
                else
                    attempt++;

                Vector3 currentPos = Aibehaviour.CurrentPosition;
                currentPos.x += RND();
                currentPos.z += RND();
                currentPos.y = GetGroundYValue(currentPos);
                CurrentPos = currentPos;

            } while (!NavMesh.CalculatePath(Aibehaviour.CurrentPosition, CurrentPos, -1, _path) || _path.status != NavMeshPathStatus.PathComplete);

            if (!_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
                yield break;

            _agent.SetPath(_path);

            _entity.VisualHandler.AnimatorController.SetFloat("Forward", .5f);
            float RND()
            {
                int radius = System.Convert.ToInt32(Aibehaviour.AIBehaviourSO.TargetDestinationRadius);
                return rnd.Next(-radius, radius);
            }
        }

        private float GetGroundYValue(Vector3 currentPos)
        {
            currentPos.y += HEIGHT_OFFSET;
            Ray rei = new Ray(currentPos, Vector3.down);
            if (Physics.Raycast(rei, out var hit, 100f, -1))
                return hit.point.y;
            else
                return currentPos.y - HEIGHT_OFFSET;
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
        //    Debug.Log($"{Aibehaviour.gameObject.name} is Running!");
            _movementSpeed.Value += _runAwaySpeed.Value;
       
        }

        public override void OnStateExit()
        {
            _movementSpeed.Value -= _runAwaySpeed.Value;
         //   Debug.Log($"{Aibehaviour.gameObject.name} is stopped running!");
            base.OnStateExit();
        }

    }
}