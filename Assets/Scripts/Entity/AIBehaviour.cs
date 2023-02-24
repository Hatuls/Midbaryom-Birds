using Midbaryom.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Midbaryom.AI
{

    public class AIBehaviour : MonoBehaviour
    {
        [SerializeField]
        private TargetedBehaviour _targetedBehaviour;
        [SerializeField]
        private NavMeshAgent _agent;
        [SerializeField]
        private AIBehaviourSO _aIBehaviourSO;
        [SerializeField]
        private Entity _entity;
        private IStateMachine _stateMachine;
        private AIBrain _aIBrain;

        private AIMoveState _moveState;
        
        public Vector3 MoveToPosition;
        public IEntity Entity => _entity;
        public AIBrain AIBrain => _aIBrain;

        public Vector3 CurrentPosition;

        
        public IEnumerable<IUpdateable> Updateables
        {
            get
            {
                yield return _stateMachine;
                yield return _aIBrain;
            }
        }

        public AIBehaviourSO AIBehaviourSO { get => _aIBehaviourSO; }

        private void Start()
        {

            _moveState = new AIMoveState(this, _agent);
            BaseState[] AIStates = new BaseState[]
            {
            new AIIdleState(this,_agent),
            _moveState,
             new AIRunAwayState(this, _agent),
            };

            IStat movementSpeedStat = Entity.StatHandler[StatType.MovementSpeed];
            movementSpeedStat.OnValueChanged += UpdateAgentSpeed;
            Entity.MovementHandler.StopMovement = true;
            UpdateAgentSpeed( movementSpeedStat.Value);


            _stateMachine = new StateMachine(StateType.Idle, AIStates);
    
            _aIBrain = new AIBrain(_stateMachine, AIBehaviourSO,_targetedBehaviour);

            float val = Random.Range(0, 360);
            _entity.Rotator.SetRotation(Quaternion.Euler(0, val, 0));

     
        }
        private void OnDestroy()
        {
            Entity.StatHandler[StatType.MovementSpeed].OnValueChanged-= UpdateAgentSpeed;

        }
        private void OnEnable()
        {
            float val = Random.Range(0, 360);
            _entity.Rotator?.SetRotation(Quaternion.Euler(0, val, 0));


            _entity.VisualHandler.AnimatorController.Animator.SetFloat("Forward", 0);
            if (_stateMachine != null)
                _stateMachine.ChangeState(StateType.Roam);
        }
        private void Update()
        {
            CurrentPosition = transform.position;
            foreach (var item in Updateables)
                item.Tick();
        }

 
        private void OnDrawGizmosSelected()
        {
            Vector3 CurrentPos = transform.position;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(CurrentPos, AIBehaviourSO.TargetDestinationRadius);
            if (_moveState != null)
            {
                MoveToPosition = _moveState.CurrentPos;
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(MoveToPosition, 2f);
            }
        }

        private void UpdateAgentSpeed(float amount)
        {
            _agent.speed = amount;

        }

    }
    public class AIBrain : IUpdateable
    {
        private readonly IStateMachine _stateMachine;
        private readonly AIBehaviourSO aIBehaviourSO;
        private readonly ITargetBehaviour _targetedBehaviour;
        private float _counter;
        private float _duration;
        public AIBrain(IStateMachine state, AIBehaviourSO aIBehaviourSO,ITargetBehaviour targetBehaviour)
        {
            _stateMachine = state;
            this.aIBehaviourSO = aIBehaviourSO;
            _targetedBehaviour = targetBehaviour;
            _targetedBehaviour.OnPotentiallyTargeted += MoveToRunAwayState;
            _targetedBehaviour.OnUnTargeted += MoveToIdleState;
            Reset();
        }

        ~AIBrain()
        {
            _targetedBehaviour.OnPotentiallyTargeted -= MoveToRunAwayState;
            _targetedBehaviour.OnUnTargeted          -= MoveToIdleState;
        }

        private void Reset()
        {
            _counter = 0;

            var currentState = _stateMachine.CurrentStateType;
            if (currentState == StateType.Idle)
                _duration = aIBehaviourSO.GetConfig(StateType.Idle).RandomDuration;
            else if (currentState == StateType.Roam)
                _duration = aIBehaviourSO.GetConfig(StateType.Roam).RandomDuration;
        }
        public void Tick()
        {
            _counter += Time.deltaTime;
            if (_counter >= _duration)
                ChangeState();
        }

        private void ChangeState()
        {
            var currentState = _stateMachine.CurrentStateType;
            if (currentState == StateType.Idle)
                _stateMachine.ChangeState(StateType.Roam);
            else if (currentState == StateType.Roam)
                _stateMachine.ChangeState(StateType.Idle);

            Reset();
        }


        private void MoveToRunAwayState()
        {
            _stateMachine.ChangeState(StateType.RunAway);
        }

        private void MoveToIdleState()
        {
            if (_stateMachine.CurrentStateType == StateType.RunAway)
                _stateMachine.ChangeState(StateType.Idle);
        }
    }
}