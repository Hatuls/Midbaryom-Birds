using System;
using System.Collections.Generic;
using UnityEngine;
namespace Midbaryom.Core
{

    public class TargetHandler : MonoBehaviour
    {
        [SerializeField]
        private Player _player;
        [SerializeField]
        private Transform _targetHoldingLocation;
        [SerializeField]
        private AimAssists _aimAssists;
        public IEntity Target => _aimAssists.Target;
        public IReadOnlyList<IEntity> AllTargets => _aimAssists.AllActiveEntities;
        private IEntity _holdingTarget;

        public bool HasTargetAttached;

        private void Awake()
        {
            //       _player.StateMachine.StateDictionary[StateType.]
            PlayerDiveState.OnTargetHit += AttachTarget;
            PlayerRecoverState.OnRecoverStateTryingToExit += ResetTarget;
        }
        private void OnDestroy()
        {
            PlayerDiveState.OnTargetHit -= AttachTarget;
            PlayerRecoverState.OnRecoverStateTryingToExit -= ResetTarget;
        }



        private void AttachTarget()
        {
            var target = Target;
            if (target == null)
                throw new Exception("Target Cannot be null");
            _holdingTarget = target;


            HasTargetAttached = true;
            _aimAssists.LockTarget();
            _player.StateMachine.ChangeState(StateType.Recover);
            _player.StateMachine.LockStateMachine = true;
            target.MovementHandler.StopMovement = true;
            target.Rotator.StopRotation = true;
            target.Transform.SetParent(_targetHoldingLocation);
            target.Transform.localPosition = Vector3.zero;
            target.Transform.rotation = Quaternion.Euler(0, 0, 0);
            target.TargetBehaviour.Targeted();
        }


        public void ResetTarget()
        {
            if (HasTargetAttached)
            {
                HasTargetAttached = false;
                _holdingTarget.DestroyHandler.Destroy();
                _aimAssists.UnLockTarget();
                _aimAssists.ResetTarget();
                _player.StateMachine.LockStateMachine = false;
                AddPoints(_holdingTarget);
                _holdingTarget = null;
            }
        }

        private void AddPoints(IEntity holdingTarget)
        {
            _player.Entity.StatHandler[StatType.Points].Value += holdingTarget.StatHandler[StatType.Points].Value;
        }

        public void LockAtTarget()
        {
            if (_aimAssists.HasTarget == false)
                return;
            // _player.StateMachine.LockStateMachine();

            // Target.TargetBehaviour.Targeted();
            _aimAssists.LockTarget();
        }


        public void UnLockTarget()
        {
            if (_aimAssists.HasTarget == false)
                return;

            Target.TargetBehaviour.UnTargeted();
            _aimAssists.UnLockTarget();
        }
    }
}