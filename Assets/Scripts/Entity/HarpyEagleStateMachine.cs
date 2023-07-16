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
            _playerDiveState = new PlayerDiveState(_player, _player.Entity.StatHandler[StatType.DiveXZSpeed]);
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
}