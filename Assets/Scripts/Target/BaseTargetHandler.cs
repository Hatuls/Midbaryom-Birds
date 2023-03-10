using System;
using System.Collections.Generic;
using UnityEngine;
namespace Midbaryom.Core
{
    public class BaseTargetHandler : MonoBehaviour
    {
        private bool _hasTargetAttached;
        protected IEntity _holdingTarget;
        protected IPlayer _player;


        [SerializeField]
        protected Transform _targetHoldingLocation;
        [SerializeField]
        protected AimAssists _aimAssists;
        public IEntity Target => _aimAssists.Target;
        public IReadOnlyList<IEntity> AllTargets => _aimAssists.AllActiveEntities;

        public bool HasTargetAttached { get => _hasTargetAttached; protected set => _hasTargetAttached = value; }

        public virtual void LockAtTarget()
        {
            if (_aimAssists.HasTarget == false)
                return;
            // _player.StateMachine.LockStateMachine();

            // Target.TargetBehaviour.Targeted();
            _aimAssists.LockTarget();
        }

        public virtual void ResetTarget()
        {
            if (HasTargetAttached)
            {
                HasTargetAttached = false;
                _holdingTarget.DestroyHandler.Destroy();
                _aimAssists.UnLockTarget();
                _aimAssists.ResetTarget();
              
                AddPoints(_holdingTarget);
                _holdingTarget.Transform.SetParent(null);
                _holdingTarget = null;
            }
        }


        public void UnLockTarget()
        {
            if (_aimAssists.HasTarget == false)
                return;

            Target.TargetBehaviour.UnTargeted();
            _aimAssists.UnLockTarget();
        }

        protected virtual void AddPoints(IEntity holdingTarget)
        {
            _player.Entity.StatHandler[StatType.Points].Value += holdingTarget.StatHandler[StatType.Points].Value;
        }

        internal virtual void InitTargetHandler(IPlayer player)
        {
            _player = player;
        }

        protected virtual void AttachTarget()
        {
     
            if (Target == null)
                throw new Exception("Target Cannot be null");

            HasTargetAttached = true;
            _holdingTarget = Target;
            _holdingTarget.TargetBehaviour.Eaten();
            _aimAssists.LockTarget();

            SetTargetAtHoldingPosition(_holdingTarget);
        }
     

        protected virtual void SetTargetAtHoldingPosition(IEntity target)
        {
            Transform targetsTransform = target.Transform;
            targetsTransform.SetParent(_targetHoldingLocation);
            var offsetPosition = target.EntityTagSO.HoldingOffset;
            targetsTransform.localPosition = offsetPosition.PositionOffset;
            targetsTransform.localRotation = offsetPosition.RotaionOffset;
        } 
      
    }

}