﻿using UnityEngine;
using UnityEngine.UI;

namespace Midbaryom.Core.Tutorial
{
    public class MoveLeft : TutorialTask
    {
        [SerializeField]
        private Spawner _spawner;
        private  IPlayer _player;
        private  OnlyLeftInputEnabled _moveLeft;
        private  float _endAngle;
        [SerializeField]
        private  float _turningAngleToComplete;
        private Transform _playerTransform;

        public override void TaskStarted()
        {
            _player = _spawner.Player; 
            _playerTransform = _player.Entity.Transform; 
             _moveLeft = new OnlyLeftInputEnabled(_player.Entity);
            _moveLeft.OnLeft += CheckTask;
       
            _player.Entity.MovementHandler.StopMovement = true;
            float currentPlayerAngle = _playerTransform.localRotation.eulerAngles.y;
            _endAngle = currentPlayerAngle + _turningAngleToComplete;
            _player.PlayerController.SetInputBehaviour(_moveLeft);

            ShowInstructions();
            StartCoroutine(EffectCoroutine(_fadeIn));
            base.TaskStarted();
        }
        void CheckTask()
        {
            if(_endAngle >= _playerTransform.localRotation.eulerAngles.y)
            {
            TaskCompleted();
            }
        }
        protected override void TaskCompleted()
        {
            _player.Entity.Rotator.AssignRotation(Vector3.zero);
            _moveLeft.OnLeft -= CheckTask;
            Debug.LogWarning("Task Left Completed");
            base.TaskCompleted();
        }
    }
}