using System;
using System.Collections.Generic;
using UnityEngine;

namespace Midbaryom.Core
{
    public class EatAllTargetHandler : BaseTargetHandler
    {
        [UnityEngine.SerializeField]
        private EatingStateTransition[] _eatingStateTransitions;


        protected override void AttachTarget()
        {
            if (Target == null)
                throw new Exception("Target Cannot be null");

            HasTargetAttached = true;
            _holdingTarget = Target;
            _holdingTarget.TargetBehaviour.Eaten();
            //_aimAssists.LockTarget();

            StateType nextState = CheckTargetsTags(_holdingTarget);
            if(nextState == StateType.Recover)
                SetTargetAtHoldingPosition(_holdingTarget);
           


            _player.StateMachine.ChangeState(nextState);
            _player.StateMachine.LockStateMachine = true;
        }

        private StateType CheckTargetsTags(IEntity holdingTarget)
        {
            for (int i = 0; i < _eatingStateTransitions.Length; i++)
            {
                if (holdingTarget.ContainTag(_eatingStateTransitions[i].TargetsTag))
                    return _eatingStateTransitions[i].GoToState;
            }

            throw new Exception("Tag was not found!");
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
            recoverState.OnRecoverStateTryingToExit += ResetTarget;
        }

        protected virtual void OnDestroy()
        {
            IReadOnlyDictionary<StateType, IState> stateDictionary = _player.StateMachine.StateDictionary;
            var diveState = stateDictionary[StateType.Dive] as PlayerDiveState;
            var recoverState = stateDictionary[StateType.Recover] as PlayerRecoverState;
            var carcasState = stateDictionary[StateType.Eat] as EatCarcassState;
            diveState.OnTargetHit -= AttachTarget;
            carcasState.OnEatDurationCompleted -= ResetTarget;
            recoverState.OnRecoverStateTryingToExit -= ResetTarget;
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




        [System.Serializable]
        public class EatingStateTransition
        {
            [UnityEngine.SerializeField]
            private TagSO _targetsTag;
            [UnityEngine.SerializeField]
            private StateType _goToState;

            public TagSO TargetsTag => _targetsTag;
            public StateType GoToState => _goToState;
        }
    }
}