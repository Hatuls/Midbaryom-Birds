using System;
using System.Collections;
using UnityEngine;
namespace Midbaryom.Core.Tutorial
{

    public class TutorialManager : MonoBehaviour
    {
        public event Action OnTutorialCompeleted;
        public event Action OnTutorialStarted;

        [SerializeField]
        private Spawner _spawner;
        [SerializeField]
        private EntityTagSO _entityTagSO;

        private BaseTutorialTask[] _baseTutorialTasks;
        private int _currentTask;
        private IPlayer _player;

        private IEnumerator Start()
        {
            yield return null;
            _player = Spawner.Instance.Player;
            InitTutorial();
        
            yield return null;
            StartTutorial();
        }

        private void InitTutorial()
        {
            _baseTutorialTasks = new BaseTutorialTask[]
            {
                new MoveLeft(_player, -20), 
                new MoveRight(_player , 40),
                new HuntTutorial(_player, _spawner,_entityTagSO)
            };
            for (int i = 0; i < _baseTutorialTasks.Length; i++)
                _baseTutorialTasks[i].OnComplete += MoveNext;
        }

        private void StartTutorial()
        {

            _currentTask = -1;
            MoveNext();
        }
        private void MoveNext()
        {
            _currentTask++;
            if (_currentTask < _baseTutorialTasks.Length)
                _baseTutorialTasks[_currentTask].TaskStarted();
            else
            {
                OnTutorialCompeleted?.Invoke();
                Debug.Log("Complete!");
            }
        }
    }


    public abstract class BaseTutorialTask
    {
        public event Action OnComplete;
        public event Action OnTaskStarted;
        protected virtual void TaskCompleted() => OnComplete?.Invoke();


        public virtual void TaskStarted() => OnTaskStarted?.Invoke();
    }

    public class HuntTutorial : BaseTutorialTask
    {
        private readonly IPlayer _player;
        private readonly Spawner _spawner;
        private readonly EntityTagSO _enemyTag;


        public HuntTutorial(IPlayer player,Spawner spawner, EntityTagSO enemyTag)
        {
            _player = player;
            _spawner = spawner;
            _enemyTag = enemyTag;
        }

        public override void TaskStarted()
        {
  _player.PlayerController.ResetToDefault();
            var mob = _spawner.SpawnEntity(_enemyTag);
            mob.DestroyHandler.OnDestroy += EntityHunted;
            //Show arrow towards enemy
            base.TaskStarted();
        }

        private void EntityHunted(IEntity entity) 
        { 
            entity.DestroyHandler.OnDestroy -= EntityHunted;
            TaskCompleted(); 
        }
    }
    public class MoveLeft : BaseTutorialTask
    {
        private readonly IPlayer _player;
        private readonly OnlyLeftInputEnabled _moveLeft;
        private  float _endAngle;
        private readonly float _turningAngleToComplete;
        private readonly Transform _playerTransform;
        public MoveLeft(IPlayer player,float turnAngleComplete)
        {
            _player = player;
            _playerTransform = _player.Entity.Transform;
                _moveLeft = new OnlyLeftInputEnabled(_player.Entity);
            _moveLeft.OnLeft += CheckTask;

            _turningAngleToComplete = turnAngleComplete;

        }

        public override void TaskStarted()
        {
            _player.Entity.MovementHandler.StopMovement = true;
            float currentPlayerAngle = _playerTransform.localRotation.eulerAngles.y;
            _endAngle = currentPlayerAngle + _turningAngleToComplete;
            _player.PlayerController.SetInputBehaviour(_moveLeft);
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

    public class MoveRight : BaseTutorialTask
    {
        private readonly IPlayer _player;
        private readonly OnlyRightInputEnabled _moveLeft;
        private float _endAngle;
        private readonly float _turningAngleToComplete;
        private readonly Transform _playerTransform;
        public MoveRight(IPlayer player, float turnAngleComplete)
        {
            _player = player;
            _playerTransform = _player.Entity.Transform;
            _moveLeft = new OnlyRightInputEnabled(_player.Entity);
            _moveLeft.OnRight += CheckTask;

            _turningAngleToComplete = turnAngleComplete;

        }

        public override void TaskStarted()
        {
            _player.Entity.MovementHandler.StopMovement = true;
            float currentPlayerAngle = _playerTransform.localRotation.eulerAngles.y;
            _endAngle = currentPlayerAngle + _turningAngleToComplete;
            _player.PlayerController.SetInputBehaviour(_moveLeft);
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
            _moveLeft.OnRight -= CheckTask;
            base.TaskCompleted();
        }
    }
}