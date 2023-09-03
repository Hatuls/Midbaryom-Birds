using System;
using UnityEngine;
using System.Collections;

namespace Midbaryom.Core
{
    public class EatCarcassState : BaseState
    {
        public event Action OnEatDurationCompleted;
        private float _counter = 0f;
        private readonly IPlayer _player;
        private readonly float _duration;
        public override StateType StateType => StateType.Eat;
        public EatCarcassState(IPlayer player,float duration) : base(player.Entity)
        {
            _player = player;
            _duration = duration;
            ResetCounter();
       
        }

        public override void OnStateEnter()
        {
            SetConfig(true);
           // _player.CameraManager.ChangeState(Camera.CameraState.Default);
            ResetCounter();
            
            _player.CameraManager.ChangeState(Camera.CameraState.LookAtCarcass);
            base.OnStateEnter();
        }

        public override void OnStateExit()
        {
            SetConfig(false);
            base.OnStateExit();
        }

        private void SetConfig(bool value)
        {
            //if (value)
            //{
            //    SoundManager.Instance.StopSound(sounds.tslila);
            //    SoundManager.Instance.CallPlaySound(sounds.GrabDeadAnimal);
            //}
            //else
            //{
            //    SoundManager.Instance.CallPlaySound(sounds.StartFlyingInGame);
            //}

            _entity.Rotator.StopRotation = value;
            _entity.MovementHandler.StopMovement = value;
            _entity.VisualHandler.AnimatorController.SetBool("IsEating", value);

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