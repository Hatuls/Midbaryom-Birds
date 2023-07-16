using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField]
        private GameObject[] _objectsToClose;

        [SerializeField]
        private RawImage _birdEye;
        [SerializeField]
        protected TransitionEffect _birdEyeFadeIn;

    
        private void Start()
        {
            _player = _spawner.Player;

        }
        public override void TaskStarted()
        {
            _player.PlayerController.LockHuntInput = true;
            Array.ForEach(_objectsToOpen, x => x.SetActive(true));
            var mob = _spawner.GetEntity(_enemyTag);
          
            mob.DestroyHandler.OnDestroy += EntityHunted;
            StartCoroutine(Fade(base.TaskStarted));
         
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
          //  _player.PlayerController.CanCancelHunt = true;
            _languageTMPRO.gameObject.SetActive(_isActive);

        }
        protected IEnumerator BirdEyeFadeOut()
        {
            float counter = 0;
            var color = _birdEye.color;
            AnimationCurve curve = _birdEyeFadeIn.Curve;
            float duration = _birdEyeFadeIn.Duration;
            while (counter <= duration)
            {
                yield return null;
                counter += Time.deltaTime;
                color.a = curve.Evaluate(counter / duration);
                _birdEye.color = color;
            }
        }
        private IEnumerator Fade(Action onComplete)
        {
            yield return BirdEyeFadeOut();
            yield return null;
            Array.ForEach(_objectsToClose, x => x.SetActive(false));
       //     yield return EffectCoroutine(_fadeOut);

            _player.PlayerController.ResetToDefault();
            _player.PlayerController.CanCancelHunt = false;
            _player.Entity.MovementHandler.StopMovement = false;

            yield return EffectCoroutine(_fadeIn);
            _player.PlayerController.LockHuntInput = false;

            _isActive = true;
            onComplete?.Invoke();
        }
    }
}