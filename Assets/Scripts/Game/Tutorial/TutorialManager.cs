using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Midbaryom.Core.Tutorial
{

    public class TutorialManager : MonoBehaviour
    {
        public event Action OnTutorialCompeleted;
        public event Action OnTutorialStarted;

     
        [SerializeField]
        private BaseTask[] _baseTutorialTasks;
        private int _currentTask;
        private IPlayer _player;

        [SerializeField]
        private LanguageTMPRO _languageTMPRO;

        private IEnumerator Start()
        {
            InitTutorial();
            yield return null;
            _player = Spawner.Instance.Player;
            yield return null;
            StartTutorial();
        }

        private void InitTutorial()
        {
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

    public class TutorialTask : BaseTask
    {

        [SerializeField, Tooltip("The picture of the movement the player need to do")]
        protected Sprite _sprite;
        [SerializeField]
        protected Image _img;
        [SerializeField]
        protected LanguageTMPRO _languageTMPRO;

        [SerializeField]
        protected int _languageIndex;

        [SerializeField]
        private GameObject _parent;
        [SerializeField]
        protected TransitionEffect _fadeIn;
        [SerializeField]
        protected TransitionEffect _fadeOut;
        protected void ShowInstructions()
        {
            _parent.SetActive(true);
            _languageTMPRO.SetText(_languageIndex);
            _img.sprite = _sprite;
            _img.SetNativeSize();
        }
        protected override void TaskCompleted()
        {
            _parent.SetActive(false);
            base.TaskCompleted();
        }
        protected IEnumerator EffectCoroutine(TransitionEffect effect, Action OnComplete = null)
        {
            float duration = effect.Duration;
            var curve = effect.Curve;
            float counter = 0;
            _img.color = effect.StartColor;
            do
            {
                yield return null;
                counter += Time.deltaTime;
                float precentage = counter / duration;

                _img.color = Color.Lerp(effect.StartColor, effect.EndColor, curve.Evaluate(precentage));

            } while (counter < duration);

            _img.color = effect.EndColor;
            yield return null;
            OnComplete?.Invoke();

        }
    }
    public abstract class BaseTask : MonoBehaviour
    {
        public event Action OnComplete;
        public event Action OnTaskStarted;
        protected virtual void TaskCompleted() => OnComplete?.Invoke();


        public virtual void TaskStarted() => OnTaskStarted?.Invoke();
    }
}