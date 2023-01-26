using Midbaryom.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Midbaryom.AI
{

    public class AIBehaviour : MonoBehaviour
    {
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
            };

   
            Entity.MovementHandler.StopMovement = true;
            _agent.speed = Entity.StatHandler[StatType.MovementSpeed].Value;
            _stateMachine = new StateMachine(StateType.Idle, AIStates);
    
            _aIBrain = new AIBrain(_stateMachine, AIBehaviourSO);

            float val = Random.Range(0, 360);
            _entity.Rotator.SetRotation(Quaternion.Euler(0, val, 0));
        }
        private void OnEnable()
        {
            float val = Random.Range(0, 360);
            _entity.Rotator?.SetRotation(Quaternion.Euler(0, val, 0));


            _entity.VisualHandler.AnimatorController.Animator.SetFloat("Forward", 0);
            if (_stateMachine != null)
                _stateMachine.ChangeState(StateType.Run);



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

    }
    public class AIBrain : IUpdateable
    {
        private readonly IStateMachine _stateMachine;
        private readonly AIBehaviourSO aIBehaviourSO;
        private float _counter;
        private float _duration;
        public AIBrain(IStateMachine state, AIBehaviourSO aIBehaviourSO)
        {
            _stateMachine = state;
            this.aIBehaviourSO = aIBehaviourSO;
            Reset();
        }

        private void Reset()
        {
            _counter = 0;

            var currentState = _stateMachine.CurrentStateType;
            if (currentState == StateType.Idle)
                _duration = aIBehaviourSO.GetConfig(StateType.Idle).RandomDuration;
            else if (currentState == StateType.Run)
                _duration = aIBehaviourSO.GetConfig(StateType.Run).RandomDuration;
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
                _stateMachine.ChangeState(StateType.Run);
            else if (currentState == StateType.Run)
                _stateMachine.ChangeState(StateType.Idle);

            Reset();
        }
    }
}