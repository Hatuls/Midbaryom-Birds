﻿using UnityEngine;
namespace Midbaryom.Core.Tutorial
{
    public class MoveRight : TutorialTask
    {
        [SerializeField]
        private Spawner _spawner;
        private  IPlayer _player;
        private  OnlyRightInputEnabled _moveRight;
        private float _endAngle;
        [SerializeField]
        private  float _turningAngleToComplete;
        private  Transform _playerTransform;


        public override void TaskStarted()
        {
            _player = _spawner.Player;
            _player.Entity.MovementHandler.StopMovement = true;
            _playerTransform = _player.Entity.Transform;
      

            _moveRight = new OnlyRightInputEnabled(_player.Entity);
            _moveRight.OnRight += CheckTask;
            float currentPlayerAngle = _playerTransform.localRotation.eulerAngles.y;
            _endAngle = currentPlayerAngle + _turningAngleToComplete;
            _player.PlayerController.SetInputBehaviour(_moveRight);

            StartCoroutine(EffectCoroutine(_fadeOut, FadeIn));

     

            void FadeIn()
            {
                ShowInstructions();
                StartCoroutine(EffectCoroutine(_fadeIn));
              
            }

            base.TaskStarted();
        }

   

        void CheckTask()
        {
            if (_endAngle <= _playerTransform.localRotation.eulerAngles.y)
                TaskCompleted();
        }
        protected override void TaskCompleted()
        {
            _player.Entity.Rotator.AssignRotation(Vector3.zero);
            Debug.LogWarning("Task Right Completed");
            _moveRight.OnRight -= CheckTask;
            _languageTMPRO.gameObject.SetActive(false);
            base.TaskCompleted();
        }
    }
}