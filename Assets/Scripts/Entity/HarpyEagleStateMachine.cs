using System;
using UnityEngine;

namespace Midbaryom.Core
{
    public class HarpyEagleStateMachine : BirdStateMachine
    {

        [SerializeField]
        private ParticleSystem _particleSystem;

        private PlayerDiveState _playerDiveState;


        public override void InitStateMachine()
        {
            _playerDiveState = new PlayerDiveState(_player, _player.Entity.StatHandler[StatType.DiveSpeed]);
            _playerDiveState.OnStateEnterEvent += _particleSystem.Play;
            _playerDiveState.OnStateExitEvent += _particleSystem.Stop;

            BaseState[] baseStates = new BaseState[]
            {
                new PlayerIdleState(_player),
                _playerDiveState,
                new PlayerRecoverState(_player,_player.Entity.StatHandler[StatType.RecoverSpeed]),
            };
            _playerStateMachine = new StateMachine(StateType.Idle, baseStates);
        }

        private void ExitState() => _playerStateMachine.ChangeState(StateType.Recover);
        public void OnDestroy()
        {
            _playerDiveState.OnStateEnterEvent -= _particleSystem.Play;
            _playerDiveState.OnStateExitEvent -= _particleSystem.Stop;
            _player.AimAssists.OnTargetReset -= ExitState;
        }
    }

    public class EatCarcassState : BaseState
    {
        public event Action OnEatDurationCompleted;
        private float _counter = 0f;
        private readonly float _duration;
        public override StateType StateType => StateType.Eat;
        public EatCarcassState(IPlayer entity,float duration) : base(entity.Entity)
        {
            _duration = duration;
            ResetCounter();
    
        }


        public override void OnStateEnter()
        {
            _entity.MovementHandler.StopMovement = true;
            ResetCounter();
            base.OnStateEnter();
        }

        public override void OnStateExit()
        {
            _entity.MovementHandler.StopMovement = false;
            base.OnStateExit();
        }

        public override void OnStateTick()
        {
            _counter += Time.deltaTime;
            base.OnStateTick();
            if (_counter >= _duration)
                OnEatDurationCompleted?.Invoke();
        }

        private void ResetCounter() => _counter = 0;
    }
}