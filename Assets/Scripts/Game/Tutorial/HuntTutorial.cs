using System;
using UnityEngine;

namespace Midbaryom.Core.Tutorial
{
    public class HuntTutorial : TutorialTask
    {
        private  IPlayer _player;
        [SerializeField]
        private  Spawner _spawner;
        [SerializeField]
        private  EntityTagSO _enemyTag;

        [SerializeField]
        private Transform _transform;

        private bool _isActive;
        [SerializeField]
        private GameObject[] _objectsToOpen;

        public override void TaskStarted()
        {
            _player = _spawner.Player;
            //   _player.Entity.Rotator.AssignRotation(Vector3.zero);
            Vector3 dir = _player.AimAssists.FacingDirection;
            Ray rei = new Ray(_player.Entity.CurrentPosition, dir);
            Physics.Raycast(rei, out RaycastHit hit);
            Array.ForEach(_objectsToOpen, x => x.SetActive(true));
            var mob = _spawner.SpawnEntity(_enemyTag, hit.point);
            mob.DestroyHandler.OnDestroy += EntityHunted;
            StartCoroutine(EffectCoroutine(_fadeOut, FadeIn));



            void FadeIn()
            {
                _player.PlayerController.ResetToDefault();
                _player.Entity.MovementHandler.StopMovement = false;

                StartCoroutine(EffectCoroutine(_fadeIn));
                base.TaskStarted();

                _isActive = true;
            }

            base.TaskStarted();
        }
        private void Update()
        {
            if (!_isActive )
                return;


            if( _player.AimAssists.HasTarget)
            {
                TargetInRange();
            }

        }

        private void TargetInRange()
        {
            _isActive = false;
            _languageTMPRO.gameObject.SetActive(true);
            ShowInstructions();

            _player.Entity.MovementHandler.StopMovement = true;
            _player.Entity.Rotator.AssignRotation(Vector3.zero);
            _player.PlayerController.SetInputBehaviour(new NoInputBehaviour());
        }

        private void EntityHunted(IEntity entity) 
        { 
            entity.DestroyHandler.OnDestroy -= EntityHunted;
            _player.PlayerController.ResetToDefault();

            TaskCompleted();
          
            _languageTMPRO.gameObject.SetActive(_isActive);

        }

    }
}