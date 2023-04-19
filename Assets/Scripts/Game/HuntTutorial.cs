using UnityEngine;
namespace Midbaryom.Core.Tutorial
{
    public class HuntTutorial : BaseTutorialTask
    {
        private  IPlayer _player;
        [SerializeField]
        private  Spawner _spawner;
        [SerializeField]
        private  EntityTagSO _enemyTag;

        [SerializeField]
        private Transform _transform;
        [SerializeField]
        private ArrowBehaviour _arrowBehaviour;

        [SerializeField]
        private LanguageTMPRO _languageTMPRO;

        [SerializeField]
        private int _languageIndex;

        private bool _isActive;
        [SerializeField]
        private float _radius;
        public override void TaskStarted()
        {
            _player = _spawner.Player;
            _player.PlayerController.ResetToDefault();
            var mob = _spawner.SpawnEntity(_enemyTag, _transform.position);
            mob.DestroyHandler.OnDestroy += EntityHunted;
            _arrowBehaviour.IsActive = true;
            _arrowBehaviour.Open();
            _arrowBehaviour.PointTowards(_player, mob);
            _player.Entity.MovementHandler.StopMovement = false;
 
            _isActive = true;


            base.TaskStarted();
        }
        private void Update()
        {
            if (!_isActive )
                return;


            if( _player.AimAssists.HasTarget)
            {
                _languageTMPRO.gameObject.SetActive(_player.AimAssists.HasTarget);
                _languageTMPRO.SetText(_languageIndex);
            }

        }
        private void EntityHunted(IEntity entity) 
        { 
            entity.DestroyHandler.OnDestroy -= EntityHunted;
            _arrowBehaviour.Close();
            _arrowBehaviour.IsActive = false;
            TaskCompleted();
            _isActive = false;
            _languageTMPRO.gameObject.SetActive(_isActive);
        }
    }
}