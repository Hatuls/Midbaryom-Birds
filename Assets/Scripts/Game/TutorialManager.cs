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
        private MoveLeft _left;
        [SerializeField]
        private MoveRight _right;
        [SerializeField]
        private HuntTutorial _hunt;

        private BaseTutorialTask[] _baseTutorialTasks;
        private int _currentTask;
        private IPlayer _player;

        [SerializeField]
        private LanguageTMPRO _languageTMPRO;

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
                _left,
                _right,
                _hunt,
            };
            for (int i = 0; i < _baseTutorialTasks.Length; i++)
                _baseTutorialTasks[i].OnComplete += MoveNext;
        }
        private void OnDestroy()
        {
            for (int i = 0; i < _baseTutorialTasks.Length; i++)
                _baseTutorialTasks[i].OnComplete -= MoveNext;
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
                if (PlayerScore.Instance != null)
                    PlayerScore.Instance.ResetScores();
                OnTutorialCompeleted?.Invoke();
                _languageTMPRO.gameObject.SetActive(false);
                Debug.Log("Complete!");
            }
        }
    }


    public abstract class BaseTutorialTask :MonoBehaviour
    {
        public event Action OnComplete;
        public event Action OnTaskStarted;
        protected virtual void TaskCompleted() => OnComplete?.Invoke();


        public virtual void TaskStarted() => OnTaskStarted?.Invoke();
    }
}