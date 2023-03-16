using System;
using UnityEngine;

namespace Midbaryom.Core
{
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
            _entity.VisualHandler.AnimatorController.SetBool("IsEating", true);
            base.OnStateEnter();
        }

        public override void OnStateExit()
        {
            _entity.MovementHandler.StopMovement = false;
            _entity.VisualHandler.AnimatorController.SetBool("IsEating", false);
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