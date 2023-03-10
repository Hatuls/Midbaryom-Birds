using System.Collections.Generic;
namespace Midbaryom.Core
{
    public class CarcassEagleTargetHandler : BaseTargetHandler
    {
        protected override void AttachTarget()
        {
            base.AttachTarget();
            _player.StateMachine.ChangeState(StateType.Eat);
            _player.StateMachine.LockStateMachine = true;
        }
        internal override void InitTargetHandler(IPlayer player)
        {
            base.InitTargetHandler(player);
            IReadOnlyDictionary<StateType, IState> stateDictionary = _player.StateMachine.StateDictionary;
            var diveState = stateDictionary[StateType.Dive] as PlayerDiveState;
            var recoverState = stateDictionary[StateType.Recover] as PlayerRecoverState;
            var carcasState = stateDictionary[StateType.Eat] as EatCarcassState;
            diveState.OnTargetHit += AttachTarget;
            carcasState.OnEatDurationCompleted += ResetTarget;
            //recoverState.OnRecoverStateTryingToExit += ResetTarget;
        }

        protected virtual void OnDestroy()
        {
            IReadOnlyDictionary<StateType, IState> stateDictionary = _player.StateMachine.StateDictionary;
            var diveState = stateDictionary[StateType.Dive] as PlayerDiveState;
            var recoverState = stateDictionary[StateType.Recover] as PlayerRecoverState;
            var carcasState = stateDictionary[StateType.Eat] as EatCarcassState;
            diveState.OnTargetHit -= AttachTarget;
            carcasState.OnEatDurationCompleted -= ResetTarget;
        }
        public override void ResetTarget()
        {
            if (HasTargetAttached)
            {
                _player.StateMachine.LockStateMachine = false;
                _player.StateMachine.ChangeState(StateType.Recover);
            }
            base.ResetTarget();
        }
    }
}